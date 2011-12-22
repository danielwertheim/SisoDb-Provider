using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Querying;
using SisoDb.Structures;

namespace SisoDb
{
	/// <summary>
	/// When targeting a Database oriented provider, e.g all providers extending <see cref="DbDatabase"/>, the infrastructure
	/// could make use of this factory to ease the interaction with <see cref="DbDatabase"/>.
	/// </summary>
	public interface IDbProviderFactory
    {
        StorageProviders ProviderType { get; }

		ISqlStatements GetSqlStatements();

        IServerClient GetServerClient(ISisoConnectionInfo connectionInfo);

        IDbClient GetTransactionalDbClient(ISisoConnectionInfo connectionInfo);

        IDbClient GetNonTransactionalDbClient(ISisoConnectionInfo connectionInfo);

        IDbSchemaManager GetDbSchemaManager();

		IStructureInserter GetStructureInserter(IDbClient dbClient);

    	IIdentityStructureIdGenerator GetIdentityStructureIdGenerator(IDbClient dbClient);

		IQueryBuilder<T> GetQueryBuilder<T>(IStructureSchemas structureSchemas) where T : class;
		
		IDbQueryGenerator GetDbQueryGenerator();
    }
}