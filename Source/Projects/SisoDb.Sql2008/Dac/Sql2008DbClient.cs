using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EnsureThat;
using NCore;
using NCore.Collections;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Core;
using SisoDb.Dac;
using SisoDb.DbSchema;

namespace SisoDb.Sql2008.Dac
{
    public class Sql2008DbClient : DbClientBase
    {
		private const int MaxBatchedIdsSize = 100;

        public Sql2008DbClient(ISisoConnectionInfo connectionInfo, IDbConnection connection, IDbTransaction transaction, IConnectionManager connectionManager, ISqlStatements sqlStatements)
            : base(connectionInfo, connection, transaction, connectionManager, sqlStatements)
        {
        }

        public override IDbBulkCopy GetBulkCopy()
        {
            return new Sql2008DbBulkCopy(Connection.ToSqlConnection(), Transaction.ToSqlTransaction());
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
    }
}