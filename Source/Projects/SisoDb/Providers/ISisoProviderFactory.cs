using SisoDb.Dac;
using SisoDb.Dac.BulkInserts;
using SisoDb.DbSchema;
using SisoDb.Querying;
using SisoDb.Querying.Sql;
using SisoDb.Structures;

namespace SisoDb.Providers
{
    public interface ISisoProviderFactory
    {
        IServerClient GetServerClient(ISisoConnectionInfo connectionInfo);
        IDbClient GetDbClient(ISisoConnectionInfo connectionInfo, bool transactional);
        IDbSchemaManager GetDbSchemaManager();
        IDbColumnGenerator GetDbColumnGenerator();
        IDbSchemaUpserter GetDbSchemaUpserter(IDbClient dbClient);
        IDbQueryGenerator GetDbQueryGenerator();
        IDbBulkInserter GetDbBulkInserter(IDbClient dbClient);
        ICommandBuilderFactory GetCommandBuilderFactory();
        IdentityStructureIdGenerator GetIdentityStructureIdGenerator(IDbClient dbClient);
    }
}