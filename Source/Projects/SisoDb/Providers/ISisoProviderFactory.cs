using System.Data;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Querying;
using SisoDb.Structures;

namespace SisoDb.Providers
{
    public interface ISisoProviderFactory
    {
        StorageProviders ProviderType { get; }

        IDbConnection GetOpenServerConnection(IConnectionString connectionString);

        void ReleaseServerConnection(IDbConnection dbConnection);

        IDbConnection GetOpenConnection(IConnectionString connectionString);

        void ReleaseConnection(IDbConnection dbConnection);

        IServerClient GetServerClient(ISisoConnectionInfo connectionInfo);

        IDbClient GetTransactionalDbClient(ISisoConnectionInfo connectionInfo);

        IDbClient GetNonTransactionalDbClient(ISisoConnectionInfo connectionInfo);

        IDbSchemaManager GetDbSchemaManager();

        ISqlStatements GetSqlStatements();
        
        IStructureInserter GetStructureInserter(IDbClient dbClient);

    	IIdentityStructureIdGenerator GetIdentityStructureIdGenerator(IDbClient dbClient);
        
        IDbQueryGenerator GetDbQueryGenerator();

		IQueryBuilder<T> GetQueryBuilder<T>(IStructureSchemas structureSchemas) where T : class;
    }
}