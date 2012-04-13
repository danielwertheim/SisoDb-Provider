using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EnsureThat;
using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Core;
using SisoDb.DbSchema;
using SisoDb.Querying.Sql;

namespace SisoDb.Dac
{
    public abstract class DbClientBase : ITransactionalDbClient
    {
        protected readonly IConnectionManager ConnectionManager;
        protected readonly ISqlStatements SqlStatements;

        public IAdoDriver Driver { get; private set; }
        public ISisoConnectionInfo ConnectionInfo { get; private set; }
        public IDbConnection Connection { get; private set; }

        public IDbTransaction Transaction { get; private set; }
        public bool Failed { get; protected set; }

        protected DbClientBase(IAdoDriver driver, ISisoConnectionInfo connectionInfo, IDbConnection connection, IDbTransaction transaction, IConnectionManager connectionManager, ISqlStatements sqlStatements)
        {
            Ensure.That(driver, "driver").IsNotNull();
            Ensure.That(connectionInfo, "connectionInfo").IsNotNull();
            Ensure.That(connection, "connection").IsNotNull();
            Ensure.That(connectionManager, "connectionManager").IsNotNull();
            Ensure.That(sqlStatements, "sqlStatements").IsNotNull();

            Driver = driver;
            ConnectionInfo = connectionInfo;
            ConnectionManager = connectionManager;
            Connection = connection;
            SqlStatements = sqlStatements;
            Transaction = transaction;
        }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);

            Exception ex = null;

            if (Transaction != null)
            {
                if (Failed)
                    Transaction.Rollback();
                else
                    Transaction.Commit();

                ex = Disposer.TryDispose(Transaction);
                Transaction = null;
            }

            if (Connection != null)
            {
                ConnectionManager.ReleaseClientDbConnection(Connection);
                Connection = null;
            }

            if (ex != null)
                throw ex;
        }

        public virtual void MarkAsFailed()
        {
            Failed = true;
        }

        public virtual IDbBulkCopy GetBulkCopy()
        {
            return new DbBulkCopy(this);
        }

        public virtual void ExecuteNonQuery(string sql, params IDacParameter[] parameters)
        {
            using (var cmd = CreateCommand(sql, parameters))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public virtual void ExecuteNonQuery(string[] sqls, params IDacParameter[] parameters)
        {
            using (var cmd = CreateCommand(string.Empty, parameters))
            {
                foreach (var sqlStatement in sqls.Where(statement => !string.IsNullOrWhiteSpace(statement)))
                {
                    cmd.CommandText = sqlStatement.Trim();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public virtual T ExecuteScalar<T>(string sql, params IDacParameter[] parameters)
        {
            using (var cmd = CreateCommand(sql, parameters))
            {
                var value = cmd.ExecuteScalar();

                if (value == null || value == DBNull.Value)
                    return default(T);

                return (T)Convert.ChangeType(value, typeof(T));
            }
        }

        public virtual void SingleResultSequentialReader(string sql, Action<IDataRecord> callback, params IDacParameter[] parameters)
        {
            using (var cmd = CreateCommand(sql, parameters))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess))
                {
                    while (reader.Read())
                    {
                        callback(reader);
                    }
                    reader.Close();
                }
            }
        }

        public virtual long CheckOutAndGetNextIdentity(string entityName, int numOfIds)
        {
            EnsureValidDbObjectName(entityName);

            var sql = SqlStatements.GetSql("Sys_Identities_CheckOutAndGetNextIdentity");

            return ExecuteScalar<long>(
                sql,
                new DacParameter(DbSchemas.Parameters.EntityNameParamPrefix, entityName),
                new DacParameter("numOfIds", numOfIds));
        }

        public virtual void RenameStructureSet(string oldStructureName, string newStructureName)
        {
            EnsureValidDbObjectName(oldStructureName);
            EnsureValidDbObjectName(newStructureName);

            var oldStructureTableName = DbSchemas.GenerateStructureTableName(oldStructureName);
            var newStructureTableName = DbSchemas.GenerateStructureTableName(newStructureName);

            var oldUniquesTableName = DbSchemas.GenerateUniquesTableName(oldStructureName);
            var newUniquesTableName = DbSchemas.GenerateUniquesTableName(newStructureName);

            var oldIndexesTableNames = new IndexesTableNames(oldStructureName);
            var newIndexesTableNames = new IndexesTableNames(newStructureName);

            if (TableExists(newStructureTableName))
                throw new SisoDbException("There allready seems to exist tables for '{0}' in the database.".Inject(newStructureTableName));

            OnBeforeRenameOfStructureSet(oldStructureTableName, oldUniquesTableName, oldIndexesTableNames);

            OnRenameStructureTable(oldStructureTableName, newStructureTableName);
            OnRenameUniquesTable(oldUniquesTableName, newUniquesTableName, oldStructureTableName, newStructureTableName);
            OnRenameIndexesTables(oldIndexesTableNames, newIndexesTableNames, oldStructureTableName, newStructureTableName);

            OnAfterRenameOfStructureSet(newStructureTableName, newUniquesTableName, newIndexesTableNames);
        }

        protected virtual void OnBeforeRenameOfStructureSet(string oldStructureTableName, string oldUniquesTableName, IndexesTableNames oldIndexesTableNames) { }

        protected virtual void OnAfterRenameOfStructureSet(string newStructureTableName, string newUniquesTableName, IndexesTableNames newIndexesTableNames) { }

        protected virtual void OnRenameStructureTable(string oldTableName, string newTableName)
        {
            using (var cmd = CreateSpCommand("sp_rename", 
                new DacParameter("objname", oldTableName), 
                new DacParameter("newname", newTableName),
                new DacParameter("objtype", "OBJECT")))
            {
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
                Driver.AddCommandParametersTo(cmd,
                    new DacParameter("objname", string.Concat("PK_", oldTableName)), 
                    new DacParameter("newname", string.Concat("PK_", newTableName)), 
                    new DacParameter("objtype", "OBJECT"));
                cmd.ExecuteNonQuery();
            }
        }

        protected virtual void OnRenameUniquesTable(string oldTableName, string newTableName, string oldStructureTableName, string newStructureTableName)
        {
            using (var cmd = CreateSpCommand("sp_rename", 
                new DacParameter("objname", oldTableName), 
                new DacParameter("newname", newTableName),
                new DacParameter("objtype", "OBJECT")))
            {
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
                Driver.AddCommandParametersTo(cmd,
                    new DacParameter("objname", string.Format("FK_{0}_{1}", oldTableName, oldStructureTableName)), 
                    new DacParameter("newname", string.Format("FK_{0}_{1}", newTableName, newStructureTableName)), 
                    new DacParameter("objtype", "OBJECT"));
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
                Driver.AddCommandParametersTo(cmd,
                    new DacParameter("objname", string.Format("{0}.UQ_{1}", newTableName, oldTableName)),
                    new DacParameter("newname", string.Concat("UQ_", newTableName)),
                    new DacParameter("objtype", "INDEX"));
                cmd.ExecuteNonQuery();
            }
        }

        protected virtual void OnRenameIndexesTables(IndexesTableNames oldIndexesTableNames, IndexesTableNames newIndexesTableNames, string oldStructureTableName, string newStructureTableName)
        {
            using (var cmd = CreateSpCommand("sp_rename"))
            {
                for(var i = 0; i < oldIndexesTableNames.AllTableNames.Length; i++)
                {
                    var oldTableName = oldIndexesTableNames[i];
                    var newTableName = newIndexesTableNames[i];

                    cmd.Parameters.Clear();
                    Driver.AddCommandParametersTo(cmd,
                        new DacParameter("objname", oldTableName),
                        new DacParameter("newname", newTableName),
                        new DacParameter("objtype", "OBJECT"));
                    cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    Driver.AddCommandParametersTo(cmd,
                        new DacParameter("objname", string.Format("FK_{0}_{1}", oldTableName, oldStructureTableName)),
                        new DacParameter("newname", string.Format("FK_{0}_{1}", newTableName, newStructureTableName)),
                        new DacParameter("objtype", "OBJECT"));
                    cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    Driver.AddCommandParametersTo(cmd,
                        new DacParameter("objname", string.Format("{0}.IX_{1}_Q", newTableName, oldTableName)),
                        new DacParameter("newname", string.Format("IX_{0}_Q", newTableName)),
                        new DacParameter("objtype", "INDEX"));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public virtual void Drop(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var names = new ModelTableNames(structureSchema);
            EnsureValidNames(names);

            var sql = SqlStatements.GetSql("DropStructureTables").Inject(
                names.IndexesTableNames.IntegersTableName,
                names.IndexesTableNames.FractalsTableName,
                names.IndexesTableNames.BooleansTableName,
                names.IndexesTableNames.DatesTableName,
                names.IndexesTableNames.GuidsTableName,
                names.IndexesTableNames.StringsTableName,
                names.IndexesTableNames.TextsTableName,
                names.UniquesTableName,
                names.StructureTableName);

            using (var cmd = CreateCommand(sql, new DacParameter(DbSchemas.Parameters.EntityNameParamPrefix, structureSchema.Name)))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public virtual void UpsertSp(string name, string createSpSql)
        {
            EnsureValidDbObjectName(name);
            Ensure.That(createSpSql, "createSpSql").IsNotNullOrWhiteSpace();

            var sql = SqlStatements.GetSql("DropSp").Inject(name);

            ExecuteNonQuery(sql);
            ExecuteNonQuery(createSpSql);
        }

        public virtual void Reset()
        {
            var tableNamesToDrop = new List<string>();
            var sql = SqlStatements.GetSql("GetTableNamesToDrop");
            var dropTableTemplate = SqlStatements.GetSql("DropTable");

            using (var cmd = CreateCommand(sql))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess))
                {
                    while (reader.Read())
                        tableNamesToDrop.Add(reader.GetString(0));
                    reader.Close();
                }

                foreach (var tableName in tableNamesToDrop)
                {
                    cmd.CommandText = dropTableTemplate.Inject(tableName);
                    cmd.ExecuteNonQuery();
                }

                cmd.CommandText = SqlStatements.GetSql("TruncateSisoDbIdentities");
                cmd.ExecuteNonQuery();
            }
        }

        public virtual void ClearQueryIndexes(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var names = new ModelTableNames(structureSchema);
            EnsureValidNames(names);

            var sql = SqlStatements.GetSql("ClearIndexesTables").Inject(
                names.IndexesTableNames.IntegersTableName,
                names.IndexesTableNames.FractalsTableName,
                names.IndexesTableNames.BooleansTableName,
                names.IndexesTableNames.DatesTableName,
                names.IndexesTableNames.GuidsTableName,
                names.IndexesTableNames.StringsTableName,
                names.IndexesTableNames.TextsTableName);

            using (var cmd = CreateCommand(sql))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public virtual void DeleteById(IStructureId structureId, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("DeleteById").Inject(structureSchema.GetStructureTableName());

            ExecuteNonQuery(sql, new DacParameter("id", structureId.Value));
        }

        public abstract void DeleteByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema);

        public virtual void DeleteByQuery(IDbQuery query, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("DeleteByQuery").Inject(
                structureSchema.GetStructureTableName(),
                query.Sql);

            ExecuteNonQuery(sql, query.Parameters.ToArray());
        }

        public virtual void DeleteIndexesAndUniquesById(IStructureId structureId, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var indexesTableNames = structureSchema.GetIndexesTableNames();
            var uniquesTableName = structureSchema.GetUniquesTableName();

            var sql = SqlStatements.GetSql("DeleteIndexesAndUniquesById").Inject(
                uniquesTableName,
                indexesTableNames.BooleansTableName,
                indexesTableNames.DatesTableName,
                indexesTableNames.FractalsTableName,
                indexesTableNames.GuidsTableName,
                indexesTableNames.IntegersTableName,
                indexesTableNames.StringsTableName,
                indexesTableNames.TextsTableName);

            ExecuteNonQuery(sql, new DacParameter("id", structureId.Value));
        }

        public virtual bool TableExists(string name)
        {
            EnsureValidDbObjectName(name);

            var sql = SqlStatements.GetSql("TableExists");
            var value = ExecuteScalar<int>(sql, new DacParameter(DbSchemas.Parameters.TableNameParamPrefix, name));

            return value > 0;
        }

        public virtual ModelTablesInfo GetModelTablesInfo(IStructureSchema structureSchema)
        {
            var names = new ModelTableNames(structureSchema);
            EnsureValidNames(names);

            return new ModelTablesInfo(names, GetModelTableStatuses(names));
        }

        public virtual ModelTableStatuses GetModelTableStatuses(ModelTableNames names)
        {
            var sql = SqlStatements.GetSql("GetModelTableStatuses");
            var parameters = names.AllTableNames.Select((n, i) => new DacParameter(DbSchemas.Parameters.TableNameParamPrefix + i, n)).ToArray();
            var matchingNames = new HashSet<string>();
            SingleResultSequentialReader(
                sql,
                dr => matchingNames.Add(dr.GetString(0)),
                parameters);

            return new ModelTableStatuses(
                matchingNames.Contains(names.StructureTableName),
                matchingNames.Contains(names.UniquesTableName),
                new IndexesTableStatuses(
                    matchingNames.Contains(names.IndexesTableNames.IntegersTableName),
                    matchingNames.Contains(names.IndexesTableNames.FractalsTableName),
                    matchingNames.Contains(names.IndexesTableNames.DatesTableName),
                    matchingNames.Contains(names.IndexesTableNames.BooleansTableName),
                    matchingNames.Contains(names.IndexesTableNames.GuidsTableName),
                    matchingNames.Contains(names.IndexesTableNames.StringsTableName),
                    matchingNames.Contains(names.IndexesTableNames.TextsTableName)));
        }

        public virtual int RowCount(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("RowCount").Inject(structureSchema.GetStructureTableName());

            return ExecuteScalar<int>(sql);
        }

        public virtual int RowCountByQuery(IStructureSchema structureSchema, IDbQuery query)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();
            Ensure.That(query, "query").IsNotNull();

            var sql = SqlStatements.GetSql("RowCountByQuery").Inject(structureSchema.GetStructureTableName(), query.Sql);

            return ExecuteScalar<int>(sql, query.Parameters.ToArray());
        }

        public virtual bool Any(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("RowCount").Inject(structureSchema.GetStructureTableName());

            return ExecuteScalar<int>(sql) > 0;
        }

        public virtual bool Any(IStructureSchema structureSchema, IDbQuery query)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();
            Ensure.That(query, "query").IsNotNull();

            var sql = SqlStatements.GetSql("RowCountByQuery").Inject(structureSchema.GetStructureTableName(), query.Sql);

            return ExecuteScalar<int>(sql, query.Parameters.ToArray()) > 0;
        }

        public virtual bool Exists(IStructureSchema structureSchema, IStructureId structureId)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();
            Ensure.That(structureId, "structureId").IsNotNull();

            var sql = SqlStatements.GetSql("ExistsById").Inject(structureSchema.GetStructureTableName());

            return ExecuteScalar<int>(sql, new DacParameter("id", structureId.Value)) > 0;
        }

        public virtual string GetJsonById(IStructureId structureId, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("GetJsonById").Inject(structureSchema.GetStructureTableName());

            return ExecuteScalar<string>(sql, new DacParameter("id", structureId.Value));
        }

        public virtual string GetJsonByIdWithLock(IStructureId structureId, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("GetJsonByIdWithLock").Inject(structureSchema.GetStructureTableName());

            return ExecuteScalar<string>(sql, new DacParameter("id", structureId.Value));
        }

        public virtual IEnumerable<string> GetJsonOrderedByStructureId(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("GetAllJson").Inject(structureSchema.GetStructureTableName());

            return YieldJson(sql);
        }

        public abstract IEnumerable<string> GetJsonByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema);

        public virtual IEnumerable<string> YieldJson(string sql, params IDacParameter[] parameters)
        {
            using (var cmd = CreateCommand(sql, parameters))
            {
                foreach (var json in YieldJson(cmd))
                    yield return json;
            }
        }

        public virtual IEnumerable<string> YieldJsonBySp(string sql, params IDacParameter[] parameters)
        {
            using (var cmd = CreateSpCommand(sql, parameters))
            {
                foreach (var json in YieldJson(cmd))
                    yield return json;
            }
        }

        public virtual void SingleInsertStructure(IStructure structure, IStructureSchema structureSchema)
        {
            var sql = SqlStatements.GetSql("SingleInsertStructure").Inject(
                structureSchema.GetStructureTableName(),
                StructureStorageSchema.Fields.Id.Name,
                StructureStorageSchema.Fields.Json.Name);

            ExecuteNonQuery(sql,
                new DacParameter(StructureStorageSchema.Fields.Id.Name, structure.Id.Value),
                new DacParameter(StructureStorageSchema.Fields.Json.Name, structure.Data));
        }

        public virtual void SingleInsertOfValueTypeIndex(IStructureIndex structureIndex, string valueTypeIndexesTableName)
        {
            EnsureValidDbObjectName(valueTypeIndexesTableName);

            var sql = SqlStatements.GetSql("SingleInsertOfValueTypeIndex").Inject(
                valueTypeIndexesTableName,
                IndexStorageSchema.Fields.StructureId.Name,
                IndexStorageSchema.Fields.MemberPath.Name,
                IndexStorageSchema.Fields.Value.Name,
                IndexStorageSchema.Fields.StringValue.Name);

            ExecuteNonQuery(sql,
                new DacParameter(IndexStorageSchema.Fields.StructureId.Name, structureIndex.StructureId.Value),
                new DacParameter(IndexStorageSchema.Fields.MemberPath.Name, structureIndex.Path),
                new DacParameter(IndexStorageSchema.Fields.Value.Name, structureIndex.Value),
                new DacParameter(IndexStorageSchema.Fields.StringValue.Name, SisoEnvironment.StringConverter.AsString(structureIndex.Value)));
        }

        public virtual void SingleInsertOfStringTypeIndex(IStructureIndex structureIndex, string stringishIndexesTableName)
        {
            EnsureValidDbObjectName(stringishIndexesTableName);

            var sql = SqlStatements.GetSql("SingleInsertOfStringTypeIndex").Inject(
                stringishIndexesTableName,
                IndexStorageSchema.Fields.StructureId.Name,
                IndexStorageSchema.Fields.MemberPath.Name,
                IndexStorageSchema.Fields.Value.Name);

            ExecuteNonQuery(sql,
                new DacParameter(IndexStorageSchema.Fields.StructureId.Name, structureIndex.StructureId.Value),
                new DacParameter(IndexStorageSchema.Fields.MemberPath.Name, structureIndex.Path),
                new DacParameter(IndexStorageSchema.Fields.Value.Name, structureIndex.Value == null ? null : structureIndex.Value.ToString()));
        }

        public virtual void SingleInsertOfUniqueIndex(IStructureIndex uniqueStructureIndex, IStructureSchema structureSchema)
        {
            var sql = SqlStatements.GetSql("SingleInsertOfUniqueIndex").Inject(
                structureSchema.GetUniquesTableName(),
                UniqueStorageSchema.Fields.StructureId.Name,
                UniqueStorageSchema.Fields.UqStructureId.Name,
                UniqueStorageSchema.Fields.UqMemberPath.Name,
                UniqueStorageSchema.Fields.UqValue.Name);

            var parameters = new DacParameter[4];
            parameters[0] = new DacParameter(UniqueStorageSchema.Fields.StructureId.Name, uniqueStructureIndex.StructureId.Value);
            parameters[1] = (uniqueStructureIndex.IndexType == StructureIndexType.UniquePerType)
                                ? new DacParameter(UniqueStorageSchema.Fields.UqStructureId.Name, DBNull.Value)
                                : new DacParameter(UniqueStorageSchema.Fields.UqStructureId.Name, uniqueStructureIndex.StructureId.Value);
            parameters[2] = new DacParameter(UniqueStorageSchema.Fields.UqMemberPath.Name, uniqueStructureIndex.Path);
            parameters[3] = new DacParameter(UniqueStorageSchema.Fields.UqValue.Name, SisoEnvironment.HashService.GenerateHash(SisoEnvironment.StringConverter.AsString(uniqueStructureIndex.Value)));

            ExecuteNonQuery(sql, parameters);
        }

        public virtual void SingleUpdateOfStructure(IStructure structure, IStructureSchema structureSchema)
        {
            var sql = SqlStatements.GetSql("SingleUpdateOfStructure").Inject(
                structureSchema.GetStructureTableName(),
                StructureStorageSchema.Fields.Json.Name,
                StructureStorageSchema.Fields.Id.Name);

            ExecuteNonQuery(sql,
                new DacParameter(StructureStorageSchema.Fields.Json.Name, structure.Data),
                new DacParameter(StructureStorageSchema.Fields.Id.Name, structure.Id.Value));
        }

        private IEnumerable<string> YieldJson(IDbCommand cmd)
        {
            Func<IDataRecord, IDictionary<int, string>, string> read = (dr, af) => dr.GetString(0);
            IDictionary<int, string> additionalJsonFields = null;

            using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess))
            {
                if (reader.Read())
                {
                    if (reader.FieldCount > 1)
                    {
                        additionalJsonFields = GetAdditionalJsonFields(reader);
                        if (additionalJsonFields.Count > 0)
                            read = GetMergedJsonStructure;
                    }
                    yield return read.Invoke(reader, additionalJsonFields);
                }

                while (reader.Read())
                {
                    yield return read.Invoke(reader, additionalJsonFields);
                }
                reader.Close();
            }
        }

        private static IDictionary<int, string> GetAdditionalJsonFields(IDataRecord dataRecord)
        {
            var indices = new Dictionary<int, string>();
            for (var i = 1; i < dataRecord.FieldCount; i++)
            {
                var name = dataRecord.GetName(i);
                if (name.Contains(StructureStorageSchema.Fields.Json.Name))
                    indices.Add(i, name);
                else
                    break;
            }
            return indices;
        }

        private static string GetMergedJsonStructure(IDataRecord dataRecord, IDictionary<int, string> additionalJsonFields)
        {
            var sb = new StringBuilder();
            sb.Append(dataRecord.GetString(0));
            sb = sb.Remove(sb.Length - 1, 1);

            foreach (var childJson in ReadChildJson(dataRecord, additionalJsonFields))
            {
                sb.Append(",");
                sb.Append(childJson);
            }

            sb.Append("}");

            return sb.ToString();
        }

        private static IEnumerable<string> ReadChildJson(IDataRecord dataRecord, IEnumerable<KeyValuePair<int, string>> additionalJsonFields)
        {
            const string jsonMemberFormat = "\"{0}\":{1}";

            return additionalJsonFields
                .Where(additionalJsonField => !dataRecord.IsDBNull(additionalJsonField.Key))
                .Select(additionalJsonField => string.Format(jsonMemberFormat,
                    additionalJsonField.Value.Replace(StructureStorageSchema.Fields.Json.Name, string.Empty),
                    dataRecord.GetString(additionalJsonField.Key)));
        }

        protected virtual void EnsureValidNames(ModelTableNames names)
        {
            foreach (var tableName in names.AllTableNames)
                EnsureValidDbObjectName(tableName);
        }

        protected virtual void EnsureValidDbObjectName(string dbObjectName)
        {
            DbObjectNameValidator.EnsureValid(dbObjectName);
        }

        protected virtual IDbCommand CreateCommand(string sql, params IDacParameter[] parameters)
        {
            return Driver.CreateCommand(Connection, sql, Transaction, parameters);
        }

        protected virtual IDbCommand CreateSpCommand(string spName, params IDacParameter[] parameters)
        {
            return Driver.CreateSpCommand(Connection, spName, Transaction, parameters);
        }
    }
}