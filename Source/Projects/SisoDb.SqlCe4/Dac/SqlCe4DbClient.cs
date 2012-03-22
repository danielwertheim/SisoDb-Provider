using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
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

        public SqlCe4DbClient(ISisoConnectionInfo connectionInfo, IDbConnection connection, IDbTransaction transaction, IConnectionManager connectionManager, ISqlStatements sqlStatements)
            : base(connectionInfo, connection, transaction, connectionManager, sqlStatements)
        {
        }

        public override IDbBulkCopy GetBulkCopy()
        {
            return new SqlCe4DbBulkCopy((SqlCeConnection)Connection, (SqlCeTransaction)Transaction);
        }

        public override void ExecuteNonQuery(string sql, params IDacParameter[] parameters)
        {
            if (!sql.Contains(";"))
                base.ExecuteNonQuery(sql, parameters);
            else
                Connection.ExecuteNonQuery(sql.Split(';'), Transaction, parameters);
        }

        public override long CheckOutAndGetNextIdentity(string entityName, int numOfIds)
        {
            Ensure.That(entityName, "entityName").IsNotNullOrWhiteSpace();

            var nextId = ExecuteScalar<long>(SqlStatements.GetSql("Sys_Identities_GetNext"), new DacParameter("entityName", entityName));

            ExecuteNonQuery(SqlStatements.GetSql("Sys_Identities_Increase"),
                new DacParameter("entityName", entityName),
                new DacParameter("numOfIds", numOfIds));

            return nextId;
        }

        protected override void OnBeforeRenameOfStructureSet(string oldStructureTableName, string oldUniquesTableName, IndexesTableNames oldIndexesTableNames)
        {
            var dropFkContraintSqlFormat = SqlStatements.GetSql("DropFkContraint");

            ExecuteNonQuery(dropFkContraintSqlFormat.Inject(oldUniquesTableName, oldStructureTableName));
            
            foreach (var oldIndexTableName in oldIndexesTableNames.AllTableNames)
                ExecuteNonQuery(dropFkContraintSqlFormat.Inject(oldIndexTableName, oldStructureTableName));
        }

        protected override void OnAfterRenameOfStructureSet(string newStructureTableName, string newUniquesTableName, IndexesTableNames newIndexesTableNames)
        {
            var addFkContraintSqlFormat = SqlStatements.GetSql("AddFkContraintAgainstStructureId");

            ExecuteNonQuery(addFkContraintSqlFormat.Inject(newUniquesTableName, newStructureTableName));

            foreach (var newIndexTableName in newIndexesTableNames.AllTableNames)
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
                for (var i = 0; i < oldIndexesTableNames.AllTableNames.Length; i++)
                {
                    var oldTableName = oldIndexesTableNames[i];
                    var newTableName = newIndexesTableNames[i];

                    cmd.Parameters.Clear();
                    cmd.AddParameters(
                        new DacParameter("objname", oldTableName),
                        new DacParameter("newname", newTableName),
                        new DacParameter("objtype", "OBJECT"));
                    cmd.CommandText = "sp_rename @objname=@objname, @newname=@newname, @objtype=@objtype";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public override void Drop(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var indexesTableNames = structureSchema.GetIndexesTableNames();
            var indexesTableStatuses = GetIndexesTableStatuses(indexesTableNames);
            var uniquesTableExists = TableExists(structureSchema.GetUniquesTableName());
            var structureTableExists = TableExists(structureSchema.GetStructureTableName());

            var sqlDropTableFormat = SqlStatements.GetSql("DropTable");

            using (var cmd = CreateCommand(string.Empty, new DacParameter("entityName", structureSchema.Name)))
            {
                DropIndexesTables(cmd, indexesTableStatuses);

                if (uniquesTableExists)
                {
                    cmd.CommandText = sqlDropTableFormat.Inject(structureSchema.GetUniquesTableName());
                    cmd.ExecuteNonQuery();
                }

                if (structureTableExists)
                {
                    cmd.CommandText = sqlDropTableFormat.Inject(structureSchema.GetStructureTableName());
                    cmd.ExecuteNonQuery();
                }

                cmd.CommandText = SqlStatements.GetSql("DeleteStructureFromSisoDbIdentitiesTable");
                cmd.ExecuteNonQuery();
            }
        }

        private void DropIndexesTables(IDbCommand cmd, IndexesTableStatuses indexesTableStatuses)
        {
            var sqlDropTableFormat = SqlStatements.GetSql("DropTable");

            if (indexesTableStatuses.IntegersTableExists)
            {
                cmd.CommandText = sqlDropTableFormat.Inject(indexesTableStatuses.Names.IntegersTableName);
                cmd.ExecuteNonQuery();
            }

            if (indexesTableStatuses.FractalsTableExists)
            {
                cmd.CommandText = sqlDropTableFormat.Inject(indexesTableStatuses.Names.FractalsTableName);
                cmd.ExecuteNonQuery();
            }

            if (indexesTableStatuses.BooleansTableExists)
            {
                cmd.CommandText = sqlDropTableFormat.Inject(indexesTableStatuses.Names.BooleansTableName);
                cmd.ExecuteNonQuery();
            }

            if (indexesTableStatuses.DatesTableExists)
            {
                cmd.CommandText = sqlDropTableFormat.Inject(indexesTableStatuses.Names.DatesTableName);
                cmd.ExecuteNonQuery();
            }

            if (indexesTableStatuses.GuidsTableExists)
            {
                cmd.CommandText = sqlDropTableFormat.Inject(indexesTableStatuses.Names.GuidsTableName);
                cmd.ExecuteNonQuery();
            }

            if (indexesTableStatuses.StringsTableExists)
            {
                cmd.CommandText = sqlDropTableFormat.Inject(indexesTableStatuses.Names.StringsTableName);
                cmd.ExecuteNonQuery();
            }

            if (indexesTableStatuses.TextsTableExists)
            {
                cmd.CommandText = sqlDropTableFormat.Inject(indexesTableStatuses.Names.TextsTableName);
                cmd.ExecuteNonQuery();
            }
        }

        public override void ClearQueryIndexes(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sqlFormat = SqlStatements.GetSql("EmptyTable");
            var indexesTables = structureSchema.GetIndexesTableNames();

            using(var cmd = CreateCommand(null))
            {
                foreach (var tableName in indexesTables.AllTableNames)
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
                    cmd.AddParameters(batchedIds);

                    var paramsString = string.Join(",", batchedIds.Select(p => string.Concat("@", p.Name)));
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
                    cmd.AddParameters(batchedIds);

                    var paramsString = string.Join(",", batchedIds.Select(p => string.Concat("@", p.Name)));
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