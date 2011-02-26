using System;
using SisoDb.Providers.SqlProvider.DbSchema;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider
{
    public abstract class SqlStructureSetProcessor<TOld, TNew>
        where TOld : class
        where TNew : class
    {
        [Serializable]
        public enum ProcessorStatuses
        {
            Keep,
            Delete,
            Cancel
        }

        protected ISisoConnectionInfo ConnectionInfo { get; private set; }

        protected IStructureSchema StructureSchema { get; private set; }
        
        protected SqlStructureSetProcessor(ISisoConnectionInfo connectionInfo, IStructureSchema structureSchema)
        {
            ConnectionInfo = connectionInfo.AssertNotNull("connectionInfo");
            StructureSchema = structureSchema.AssertNotNull("structureSchema");
        }

        public void Process()
        {
            using (var dbClient = new SqlDbClient(ConnectionInfo, true))
            {
                var upserter = new SqlDbSchemaUpserter(dbClient);
                upserter.Upsert(StructureSchema);

                var sql = dbClient.SqlStringsRepository.GetSql("GetAll").Inject(StructureSchema.GetStructureTableName());
            }
        }

        protected abstract ProcessorStatuses OnProcess(TOld oldItem, TNew package);
    }
}