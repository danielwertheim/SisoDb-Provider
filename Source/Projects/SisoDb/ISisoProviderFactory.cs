using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Querying;
using SisoDb.Structures;

namespace SisoDb
{
	public interface ISisoProviderFactory //TODO: DbProviderFactory
    {
        StorageProviders ProviderType { get; }

        IServerClient GetServerClient(ISisoConnectionInfo connectionInfo);

        IDbClient GetTransactionalDbClient(ISisoConnectionInfo connectionInfo);

        IDbClient GetNonTransactionalDbClient(ISisoConnectionInfo connectionInfo);

        IDbSchemaManager GetDbSchemaManager();

		IStructureInserter GetStructureInserter(IDbClient dbClient);

    	IIdentityStructureIdGenerator GetIdentityStructureIdGenerator(IDbClient dbClient);
        
        IDbQueryGenerator GetDbQueryGenerator();

		IQueryBuilder<T> GetQueryBuilder<T>(IStructureSchemas structureSchemas) where T : class;
    }
}