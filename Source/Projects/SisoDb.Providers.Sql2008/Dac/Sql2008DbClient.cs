using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using EnsureThat;
using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Core;
using SisoDb.Dac;
using SisoDb.Querying.Sql;
using SisoDb.Structures;

namespace SisoDb.Sql2008.Dac
{
    public class Sql2008DbClient : DbClientBase
    {
		private const int MaxBatchedIdsSize = 100;

		public Sql2008DbClient(ISisoConnectionInfo connectionInfo, bool transactional, IConnectionManager connectionManager, ISqlStatements sqlStatements)
            : base(connectionInfo, transactional, connectionManager, sqlStatements)
        {
        }

        public override IDbBulkCopy GetBulkCopy()
        {
            return new Sql2008DbBulkCopy((SqlConnection)Connection, SqlBulkCopyOptions.Default, (SqlTransaction)Transaction);
        }

        public override void Drop(IStructureSchema structureSchema)
        {
            var sql = SqlStatements.GetSql("DropStructureTables").Inject(
                structureSchema.GetIndexesTableName(),
                structureSchema.GetUniquesTableName(),
                structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(sql, new DacParameter("entityHash", structureSchema.Hash)))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public override void RefreshIndexes(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("RefreshIndexes").Inject(
                structureSchema.GetStructureTableName(),
                structureSchema.GetIndexesTableName(),
                structureSchema.GetUniquesTableName());

            using (var cmd = CreateCommand(sql))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public override void DeleteById(IStructureId structureId, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("DeleteById").Inject(structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(sql, new DacParameter("id", structureId.Value)))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public override void DeleteByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("DeleteByIds").Inject(structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(sql))
            {
            	foreach (var idBatch in ids.Batch(MaxBatchedIdsSize))
            	{
					cmd.Parameters.Clear();
					cmd.Parameters.Add(Sql2008IdsTableParam.CreateIdsTableParam(structureSchema.IdAccessor.IdType, idBatch));
					cmd.ExecuteNonQuery();	
            	}
            }
        }

        public override void DeleteByQuery(SqlQuery query, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("DeleteByQuery").Inject(
                structureSchema.GetStructureTableName(),
                query.Sql);

            using (var cmd = CreateCommand(sql, query.Parameters.ToArray()))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public override void DeleteWhereIdIsBetween(IStructureId structureIdFrom, IStructureId structureIdTo, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("DeleteWhereIdIsBetween").Inject(structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(sql, new DacParameter("idFrom", structureIdFrom.Value), new DacParameter("idTo", structureIdTo.Value)))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public override bool TableExists(string name)
        {
            Ensure.That(name, "name").IsNotNullOrWhiteSpace();

            var sql = SqlStatements.GetSql("TableExists");
            var value = ExecuteScalar<string>(sql, new DacParameter("tableName", name));

            return !string.IsNullOrWhiteSpace(value);
        }

        public override int RowCount(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("RowCount").Inject(structureSchema.GetStructureTableName());

            return ExecuteScalar<int>(sql);
        }

        public override int RowCountByQuery(IStructureSchema structureSchema, SqlQuery query)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("RowCountByQuery").Inject(structureSchema.GetStructureTableName(), query.Sql);

            return ExecuteScalar<int>(sql, query.Parameters.ToArray());
        }

        public override long CheckOutAndGetNextIdentity(string entityHash, int numOfIds)
        {
            Ensure.That(entityHash, "entityHash").IsNotNullOrWhiteSpace();

            var sql = SqlStatements.GetSql("Sys_Identities_CheckOutAndGetNextIdentity");

            return ExecuteScalar<long>(sql,
                new DacParameter("entityHash", entityHash),
                new DacParameter("numOfIds", numOfIds));
        }

        public override string GetJsonById(IStructureId structureId, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("GetJsonById").Inject(structureSchema.GetStructureTableName());

            return ExecuteScalar<string>(sql, new DacParameter("id", structureId.Value));
        }

        public override IEnumerable<string> GetJsonByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("GetJsonByIds").Inject(structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(sql))
            {
				foreach (var idBatch in ids.Batch(MaxBatchedIdsSize))
				{
					cmd.Parameters.Clear();
					cmd.Parameters.Add(Sql2008IdsTableParam.CreateIdsTableParam(structureSchema.IdAccessor.IdType, idBatch));

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

        public override IEnumerable<string> GetJsonWhereIdIsBetween(IStructureId structureIdFrom, IStructureId structureIdTo, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("GetJsonWhereIdIsBetween").Inject(structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(sql, new DacParameter("idFrom", structureIdFrom.Value), new DacParameter("idTo", structureIdTo.Value)))
            {
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