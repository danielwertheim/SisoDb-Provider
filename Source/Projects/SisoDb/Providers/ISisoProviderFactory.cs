using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Dac.BulkInserts;
using SisoDb.DbSchema;
using SisoDb.Querying;
using SisoDb.Structures;

namespace SisoDb.Providers
{
    public interface ISisoProviderFactory
    {
        IServerClient GetServerClient(ISisoConnectionInfo connectionInfo);
        IDbClient GetDbClient(ISisoConnectionInfo connectionInfo, bool transactional);
        IDbSchemaManager GetDbSchemaManager();
        IDbSchemaUpserter GetDbSchemaUpserter(IDbClient dbClient);
        IDbQueryGenerator GetDbQueryGenerator();
        IDbBulkInserter GetDbBulkInserter(IDbClient dbClient);
        IGetCommandBuilder<T> CreateGetCommandBuilder<T>() where T : class;
        IQueryCommandBuilder<T> CreateQueryCommandBuilder<T>(IStructureSchema structureSchema) where T : class;
        IdentityStructureIdGenerator GetIdentityStructureIdGenerator(IDbClient dbClient);
        ISqlStatements GetSqlStatements();
    }
}