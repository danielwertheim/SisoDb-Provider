using System;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Querying;
using SisoDb.Querying.Sql;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

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
        IDbSchemas GetDbSchemaManagerFor(ISisoDatabase db);
        IStructureInserter GetStructureInserter(IDbClient dbClient);
        IStructureIdGenerator GetGuidStructureIdGenerator();
        IIdentityStructureIdGenerator GetIdentityStructureIdGenerator(IDbClient dbClient);
        IQueryBuilder GetQueryBuilder(Type structureType, IStructureSchemas structureSchemas);
        IQueryBuilder<T> GetQueryBuilder<T>(IStructureSchemas structureSchemas) where T : class;
        ISqlExpressionBuilder GetSqlExpressionBuilder();
        IDbQueryGenerator GetDbQueryGenerator();
        INamedQueryGenerator<T> GetNamedQueryGenerator<T>(IStructureSchemas structureSchemas) where T : class;
    }
}