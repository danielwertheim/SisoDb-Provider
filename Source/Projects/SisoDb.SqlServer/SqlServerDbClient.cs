using System.Collections.Generic;
using System.Data;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.EnsureThat;
using SisoDb.NCore;
using SisoDb.NCore.Collections;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.SqlServer
{
    public class SqlServerDbClient : DbClientBase
    {
        protected virtual int MaxBatchedIdsSize
        {
            get { return 100; }
        }

        public SqlServerDbClient(IAdoDriver driver, IDbConnection connection, IConnectionManager connectionManager, ISqlStatements sqlStatements)
            : base(driver, connection, connectionManager, sqlStatements)
        {
        }

        public SqlServerDbClient(IAdoDriver driver, IDbConnection connection, IDbTransaction transaction, IConnectionManager connectionManager, ISqlStatements sqlStatements)
            : base(driver, connection, transaction, connectionManager, sqlStatements)
        {
        }

        protected override IDbBulkCopy GetBulkCopy()
        {
            return new SqlServerBulkCopy(this);
        }

        public override void DeleteAllExceptIds(IEnumerable<IStructureId> structureIds, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("DeleteAllExceptIds").Inject(structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(sql))
            {
                foreach (var idBatch in structureIds.Batch(MaxBatchedIdsSize))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(SqlServerIdsTableParam.CreateIdsTableParam(structureSchema.IdAccessor.IdType, idBatch));
                    cmd.ExecuteNonQuery();
                }
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
					cmd.Parameters.Add(SqlServerIdsTableParam.CreateIdsTableParam(structureSchema.IdAccessor.IdType, idBatch));
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
                    cmd.Parameters.Add(SqlServerIdsTableParam.CreateIdsTableParam(structureSchema.IdAccessor.IdType, idBatch));

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