using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using EnsureThat;
using ErikEJ.SqlCe;
using NCore;
using NCore.Collections;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Querying.Sql;

namespace SisoDb.SqlCe4.Dac
{
    public class SqlCe4DbClient : DbClientBase
    {
    	private const int MaxBatchedIdsSize = 32;

		public SqlCe4DbClient(ISisoConnectionInfo connectionInfo, bool transactional, IConnectionManager connectionManager, ISqlStatements sqlStatements)
            : base(connectionInfo, transactional, connectionManager, sqlStatements)
        {
        }

        public override void ExecuteNonQuery(string sql, params IDacParameter[] parameters)
        {
            if (!sql.Contains(";"))
                base.ExecuteNonQuery(sql, parameters);
            else
                Connection.ExecuteNonQuery(Transaction, sql.Split(';'), parameters);
        }

        public override IDbBulkCopy GetBulkCopy()
        {
            return new SqlCe4DbBulkCopy((SqlCeConnection)Connection, SqlCeBulkCopyOptions.Default, (SqlCeTransaction)Transaction);
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

        public override void DeleteById(IStructureId structureId, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("DeleteById").Inject(structureSchema.GetStructureTableName());

            ExecuteNonQuery(sql, new DacParameter("id", structureId.Value));
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

        public override void DeleteByQuery(DbQuery query, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("DeleteByQuery").Inject(
                structureSchema.GetStructureTableName(),
                query.Sql);

            ExecuteNonQuery(sql, query.Parameters.ToArray());
        }

        public override void DeleteWhereIdIsBetween(IStructureId structureIdFrom, IStructureId structureIdTo, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("DeleteWhereIdIsBetween").Inject(structureSchema.GetStructureTableName());

            ExecuteNonQuery(sql, new DacParameter("idFrom", structureIdFrom.Value), new DacParameter("idTo", structureIdTo.Value));
        }

        public override bool TableExists(string name)
        {
            Ensure.That(name, "name").IsNotNullOrWhiteSpace();

            var sql = SqlStatements.GetSql("TableExists");
            var value = ExecuteScalar<int>(sql, new DacParameter("tableName", name));

            return value > 0;
        }

        public override int RowCount(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("RowCount").Inject(structureSchema.GetStructureTableName());

            return ExecuteScalar<int>(sql);
        }

        public override int RowCountByQuery(IStructureSchema structureSchema, DbQuery query)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("RowCountByQuery").Inject(structureSchema.GetStructureTableName(), query.Sql);

            return ExecuteScalar<int>(sql, query.Parameters.ToArray());
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