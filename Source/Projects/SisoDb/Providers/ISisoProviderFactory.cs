using System.Data;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Dac.BulkInserts;
using SisoDb.DbSchema;
using SisoDb.Querying;

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
        
        IDbSchemaUpserter GetDbSchemaUpserter(IDbClient dbClient);

        ISqlStatements GetSqlStatements();
        
        IDbStructureInserter GetDbStructureInserter(IDbClient dbClient);
        
        IDbQueryGenerator GetDbQueryGenerator();
        
        IGetCommandBuilder<T> CreateGetCommandBuilder<T>() where T : class;
        
        IQueryCommandBuilder<T> CreateQueryCommandBuilder<T>(IStructureSchema structureSchema) where T : class;
    }
}