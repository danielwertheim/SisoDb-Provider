using System;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Querying;
using SisoDb.Querying.Sql;
using SisoDb.Structures;

namespace SisoDb
{
	public interface IDbProviderFactory
    {
        StorageProviders ProviderType { get; }

	    IConnectionManager ConnectionManager { get; }
    
	    IAdoDriver GetAdoDriver();

	    IDbSettings GetSettings();

	    ISqlStatements GetSqlStatements();

        IServerClient GetServerClient(ISisoConnectionInfo connectionInfo);

        ITransactionalDbClient GetTransactionalDbClient(ISisoConnectionInfo connectionInfo);

        IDbClient GetNonTransactionalDbClient(ISisoConnectionInfo connectionInfo);

        IDbSchemaManager GetDbSchemaManagerFor(ISisoDatabase db);

		IStructureInserter GetStructureInserter(IDbClient dbClient);

    	IIdentityStructureIdGenerator GetIdentityStructureIdGenerator(CheckOutAngGetNextIdentity action);

		IQueryBuilder GetQueryBuilder(Type structureType, IStructureSchemas structureSchemas);

        IQueryBuilder<T> GetQueryBuilder<T>(IStructureSchemas structureSchemas) where T : class;

	    ISqlExpressionBuilder GetSqlExpressionBuilder();

        IDbQueryGenerator GetDbQueryGenerator();

        INamedQueryGenerator<T> GetNamedQueryGenerator<T>(IStructureSchemas structureSchemas) where T : class;
    }
}