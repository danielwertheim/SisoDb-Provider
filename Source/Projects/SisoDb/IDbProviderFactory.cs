using System;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Querying;
using SisoDb.Structures;

namespace SisoDb
{
	/// <summary>
	/// When targeting a Database oriented provider, e.g all providers extending <see cref="SisoDatabase"/>, the infrastructure
	/// could make use of this factory to ease the interaction with <see cref="SisoDatabase"/>.
	/// </summary>
	public interface IDbProviderFactory
    {
        StorageProviders ProviderType { get; }

	    IDbSettings GetSettings();

        IConnectionManager ConnectionManager { get; set; }

	    ISqlStatements GetSqlStatements();

        IServerClient GetServerClient(ISisoConnectionInfo connectionInfo);

        ITransactionalDbClient GetTransactionalDbClient(ISisoConnectionInfo connectionInfo);

        IDbClient GetNonTransactionalDbClient(ISisoConnectionInfo connectionInfo);

        IDbSchemaManager GetDbSchemaManagerFor(ISisoDatabase db);

		IStructureInserter GetStructureInserter(IDbClient dbClient);

    	IIdentityStructureIdGenerator GetIdentityStructureIdGenerator(CheckOutAngGetNextIdentity action);

		IDbQueryGenerator GetDbQueryGenerator();

	    IQueryBuilder GetQueryBuilder(Type structureType, IStructureSchemas structureSchemas);

        IQueryBuilder<T> GetQueryBuilder<T>(IStructureSchemas structureSchemas) where T : class;
    }
}