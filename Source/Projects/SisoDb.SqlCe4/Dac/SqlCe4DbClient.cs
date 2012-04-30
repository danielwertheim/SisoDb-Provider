using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using EnsureThat;
using NCore;
using NCore.Collections;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;

namespace SisoDb.SqlCe4.Dac
{
    public class SqlCe4DbClient : DbClientBase
    {
        private const int MaxBatchedIdsSize = 32;

        public SqlCe4DbClient(IAdoDriver driver, ISisoConnectionInfo connectionInfo, IDbConnection connection, IDbTransaction transaction, IConnectionManager connectionManager, ISqlStatements sqlStatements)
            : base(driver, connectionInfo, connection, transaction, connectionManager, sqlStatements)
        {
        }

        public override IDbBulkCopy GetBulkCopy()
        {
            return new SqlCe4DbBulkCopy(this);
        }

        public override void ExecuteNonQuery(string sql, params IDacParameter[] parameters)
        {
            if (!sql.Contains(";"))
                base.ExecuteNonQuery(sql, parameters);
            else
                base.ExecuteNonQuery(sql.Split(new []{';'}, StringSplitOptions.RemoveEmptyEntries), parameters);
        }

        public override long CheckOutAndGetNextIdentity(string entityName, int numOfIds)
        {
            Ensure.That(entityName, "entityName").IsNotNullOrWhiteSpace();

            var nextId = ExecuteScalar<long>(SqlStatements.GetSql("Sys_Identities_GetNext"), new DacParameter(DbSchemas.Parameters.EntityNameParamPrefix, entityName));

            ExecuteNonQuery(SqlStatements.GetSql("Sys_Identities_Increase"),
                new DacParameter(DbSchemas.Parameters.EntityNameParamPrefix, entityName),
                new DacParameter("numOfIds", numOfIds));

            return nextId;
        }

        protected override void OnBeforeRenameOfStructureSet(string oldStructureTableName, string oldUniquesTableName, IndexesTableNames oldIndexesTableNames)
        {
            var dropFkContraintSqlFormat = SqlStatements.GetSql("DropFkContraint");

            ExecuteNonQuery(dropFkContraintSqlFormat.Inject(oldUniquesTableName, oldStructureTableName));
            
            foreach (var oldIndexTableName in oldIndexesTableNames.All)
                ExecuteNonQuery(dropFkContraintSqlFormat.Inject(oldIndexTableName, oldStructureTableName));
        }

        protected override void OnAfterRenameOfStructureSet(string newStructureTableName, string newUniquesTableName, IndexesTableNames newIndexesTableNames)
        {
            var addFkContraintSqlFormat = SqlStatements.GetSql("AddFkContraintAgainstStructureId");

            ExecuteNonQuery(addFkContraintSqlFormat.Inject(newUniquesTableName, newStructureTableName));

            foreach (var newIndexTableName in newIndexesTableNames.All)
                ExecuteNonQuery(addFkContraintSqlFormat.Inject(newIndexTableName, newStructureTableName));
        }

        protected override void OnRenameStructureTable(string oldTableName, string newTableName)
        {
            var dropPkContraintSqlFormat = SqlStatements.GetSql("DropPkConstraint");
            var addPkConstraintSqlFormat = SqlStatements.GetSql("AddPkConstraintAgainstStructureId");

            ExecuteNonQuery(dropPkContraintSqlFormat.Inject(oldTableName));
            ExecuteNonQuery("sp_rename @objname=@objname, @newname=@newname, @objtype=@objtype",
                new DacParameter("objname", oldTableName),
                new DacParameter("newname", newTableName),
                new DacParameter("objtype", "OBJECT"));
            ExecuteNonQuery(addPkConstraintSqlFormat.Inject(newTableName));
        }

        protected override void OnRenameUniquesTable(string oldTableName, string newTableName, string oldStructureTableName, string newStructureTableName)
        {
            ExecuteNonQuery("sp_rename @objname=@objname, @newname=@newname, @objtype=@objtype",
                new DacParameter("objname", oldTableName),
                new DacParameter("newname", newTableName),
                new DacParameter("objtype", "OBJECT"));
        }

        protected override void OnRenameIndexesTables(IndexesTableNames oldIndexesTableNames, IndexesTableNames newIndexesTableNames, string oldStructureTableName, string newStructureTableName)
        {
            using (var cmd = CreateCommand(null))
            {
                for (var i = 0; i < oldIndexesTableNames.All.Length; i++)
                {
                    var oldTableName = oldIndexesTableNames[i];
                    var newTableName = newIndexesTableNames[i];

                    cmd.Parameters.Clear();
                    Driver.AddCommandParametersTo(cmd,
                        new DacParameter("objname", oldTableName),
                        new DacParameter("newname", newTableName),
                        new DacParameter("objtype", "OBJECT"));
                    cmd.CommandText = "sp_rename @objname=@objname, @newname=@newname, @objtype=@objtype";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected override void SetRowIdOnOrOff(string setRowIdToXForStructureTable, string setRowIdToXForIndexesAndUniquesTable)
        {
            var tableNames = new List<string>();
            var sql = SqlStatements.GetSql("GetTableNamesForAllDataTables");

            using (var cmd = CreateCommand(sql))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess))
                {
                    while (reader.Read())
                        tableNames.Add(reader.GetString(0));
                    reader.Close();
                }

                foreach (var tableName in tableNames)
                {
                    var commandText = tableName.EndsWith(DbSchemas.Suffixes.StructureTableNameSuffix) 
                        ? setRowIdToXForStructureTable.Inject(tableName) 
                        : setRowIdToXForIndexesAndUniquesTable.Inject(tableName);

                    foreach (var statement in commandText.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        cmd.CommandText = statement.Trim();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public override void Drop(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var modelInfo = GetModelTablesInfo(structureSchema);

            var sqlDropTableFormat = SqlStatements.GetSql("DropTable");

            using (var cmd = CreateCommand(string.Empty, new DacParameter(DbSchemas.Parameters.EntityNameParamPrefix, structureSchema.Name)))
            {
                DropIndexesTables(cmd, modelInfo.Statuses.IndexesTableStatuses, modelInfo.Names.IndexesTableNames);

                if (modelInfo.Statuses.UniquesTableExists)
                {
                    cmd.CommandText = sqlDropTableFormat.Inject(structureSchema.GetUniquesTableName());
                    cmd.ExecuteNonQuery();
                }

                if (modelInfo.Statuses.StructureTableExists)
                {
                    cmd.CommandText = sqlDropTableFormat.Inject(structureSchema.GetStructureTableName());
                    cmd.ExecuteNonQuery();
                }

                cmd.CommandText = SqlStatements.GetSql("DeleteStructureFromSisoDbIdentitiesTable");
                cmd.ExecuteNonQuery();
            }
        }

        private void DropIndexesTables(IDbCommand cmd, IndexesTableStatuses statuses, IndexesTableNames names)
        {
            var sqlDropTableFormat = SqlStatements.GetSql("DropTable");

            if (statuses.IntegersTableExists)
            {
                cmd.CommandText = sqlDropTableFormat.Inject(names.IntegersTableName);
                cmd.ExecuteNonQuery();
            }

            if (statuses.FractalsTableExists)
            {
                cmd.CommandText = sqlDropTableFormat.Inject(names.FractalsTableName);
                cmd.ExecuteNonQuery();
            }

            if (statuses.BooleansTableExists)
            {
                cmd.CommandText = sqlDropTableFormat.Inject(names.BooleansTableName);
                cmd.ExecuteNonQuery();
            }

            if (statuses.DatesTableExists)
            {
                cmd.CommandText = sqlDropTableFormat.Inject(names.DatesTableName);
                cmd.ExecuteNonQuery();
            }

            if (statuses.GuidsTableExists)
            {
                cmd.CommandText = sqlDropTableFormat.Inject(names.GuidsTableName);
                cmd.ExecuteNonQuery();
            }

            if (statuses.StringsTableExists)
            {
                cmd.CommandText = sqlDropTableFormat.Inject(names.StringsTableName);
                cmd.ExecuteNonQuery();
            }

            if (statuses.TextsTableExists)
            {
                cmd.CommandText = sqlDropTableFormat.Inject(names.TextsTableName);
                cmd.ExecuteNonQuery();
            }
        }

        public override void UpsertSp(string name, string createSpSql)
        {
            throw new SisoDbException("SQL CE 4 does not support stored procedures.");
        }

        public override void ClearQueryIndexes(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sqlFormat = SqlStatements.GetSql("EmptyTable");
            var indexesTables = structureSchema.GetIndexesTableNames();

            using(var cmd = CreateCommand(null))
            {
                foreach (var tableName in indexesTables.All)
                {
                    cmd.CommandText = sqlFormat.Inject(tableName);
                    cmd.ExecuteNonQuery();
                }   
            }
        }

        public override void DeleteByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();
            var sqlFormat = SqlStatements.GetSql("DeleteByIds").Inject(structureSchema.GetStructureTableName(), "{0}");

            using (var cmd = CreateCommand(string.Empty))
            {
                foreach (var batchedIds in ids.Batch<IStructureId, IDacParameter>(MaxBatchedIdsSize, (id, batchCount) => new DacParameter(string.Concat("id", batchCount), id.Value)))
                {
                    cmd.Parameters.Clear();
                    Driver.AddCommandParametersTo(cmd, batchedIds);

                    var paramsString = string.Join(",", batchedIds.Select(p => p.Name));
                    cmd.CommandText = sqlFormat.Inject(paramsString);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public override IEnumerable<string> GetJsonByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();
            var sqlFormat = SqlStatements.GetSql("GetJsonByIds").Inject(structureSchema.GetStructureTableName(), "{0}");

            using (var cmd = CreateCommand(string.Empty))
            {
                foreach (var batchedIds in ids.Batch<IStructureId, IDacParameter>(MaxBatchedIdsSize, (id, batchCount) => new DacParameter(string.Concat("id", batchCount), id.Value)))
                {
                    cmd.Parameters.Clear();
                    Driver.AddCommandParametersTo(cmd, batchedIds);

                    var paramsString = string.Join(",", batchedIds.Select(p => p.Name));
                    cmd.CommandText = sqlFormat.Inject(paramsString);

                    using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess))
                    {
                        while (reader.Read())
                        {
                            yield return reader.GetString(0);
                        }
                        reader.Close();
                    }
                }
            }
        }
    }
}