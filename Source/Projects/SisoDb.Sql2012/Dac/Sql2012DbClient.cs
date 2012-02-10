using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using EnsureThat;
using NCore;
using NCore.Collections;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Querying.Sql;

namespace SisoDb.Sql2012.Dac
{
    public class Sql2012DbClient : DbClientBase
    {
		private const int MaxBatchedIdsSize = 100;

		public Sql2012DbClient(ISisoConnectionInfo connectionInfo, IConnectionManager connectionManager, ISqlStatements sqlStatements)
            : base(connectionInfo, connectionManager, sqlStatements)
        {
        }

        public override IDbBulkCopy GetBulkCopy()
        {
            return new Sql2012DbBulkCopy((SqlConnection)Connection, this);
        }

        public override void Drop(IStructureSchema structureSchema)
        {
			Ensure.That(structureSchema, "structureSchema").IsNotNull();

        	var indexesTableNames = structureSchema.GetIndexesTableNames();

            var sql = SqlStatements.GetSql("DropStructureTables").Inject(
                indexesTableNames.IntegersTableName,
				indexesTableNames.FractalsTableName,
				indexesTableNames.BooleansTableName,
				indexesTableNames.DatesTableName,
				indexesTableNames.GuidsTableName,
				indexesTableNames.StringsTableName,
				indexesTableNames.TextsTableName,
                structureSchema.GetUniquesTableName(),
                structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(sql, new DacParameter("entityName", structureSchema.Name)))
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
					cmd.Parameters.Add(Sql2012IdsTableParam.CreateIdsTableParam(structureSchema.IdAccessor.IdType, idBatch));
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

        public override int RowCountByQuery(IStructureSchema structureSchema, DbQuery query)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("RowCountByQuery").Inject(structureSchema.GetStructureTableName(), query.Sql);

            return ExecuteScalar<int>(sql, query.Parameters.ToArray());
        }

        public override long CheckOutAndGetNextIdentity(string entityName, int numOfIds)
        {
			Ensure.That(entityName, "entityName").IsNotNullOrWhiteSpace();

            var sql = SqlStatements.GetSql("Sys_Identities_CheckOutAndGetNextIdentity");

            return ExecuteScalar<long>(sql,
				new DacParameter("entityName", entityName),
                new DacParameter("numOfIds", numOfIds));
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
					cmd.Parameters.Add(Sql2012IdsTableParam.CreateIdsTableParam(structureSchema.IdAccessor.IdType, idBatch));

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