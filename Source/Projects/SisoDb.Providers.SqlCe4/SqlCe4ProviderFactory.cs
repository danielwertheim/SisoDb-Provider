using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Dac.BulkInserts;
using SisoDb.DbSchema;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.SqlCe4.Dac;
using SisoDb.Structures;

namespace SisoDb.SqlCe4
{
	public class SqlCe4ProviderFactory : ISisoProviderFactory
    {
		private readonly IConnectionManager _connectionManager;
        private readonly ISqlStatements _sqlStatements;

        public SqlCe4ProviderFactory()
        {
			_connectionManager = new SqlCe4ConnectionManager();
            _sqlStatements = SqlCe4Statements.Instance;
        }

        public StorageProviders ProviderType
        {
            get { return StorageProviders.SqlCe4; }
        }

        public virtual IServerClient GetServerClient(ISisoConnectionInfo connectionInfo)
        {
            return new SqlCe4ServerClient((SqlCe4ConnectionInfo)connectionInfo, _connectionManager, _sqlStatements);
        }

        public IDbClient GetTransactionalDbClient(ISisoConnectionInfo connectionInfo)
        {
			return new SqlCe4DbClient(connectionInfo, true, _connectionManager, _sqlStatements);
        }

        public IDbClient GetNonTransactionalDbClient(ISisoConnectionInfo connectionInfo)
        {
			return new SqlCe4DbClient(connectionInfo, false, _connectionManager, _sqlStatements);
        }

        public virtual IDbSchemaManager GetDbSchemaManager()
        {
			return new DbSchemaManager(new SqlDbSchemaUpserter(_sqlStatements));
        }

        public virtual IStructureInserter GetStructureInserter(IDbClient dbClient)
        {
            return new DbStructureInserter(dbClient);
        }

    	public IIdentityStructureIdGenerator GetIdentityStructureIdGenerator(IDbClient dbClient)
    	{
    		return new DbIdentityStructureIdGenerator(dbClient);
    	}

    	public virtual IDbQueryGenerator GetDbQueryGenerator()
        {
			return new SqlCe4QueryGenerator(_sqlStatements);
        }

    	public IQueryBuilder<T> GetQueryBuilder<T>(IStructureSchemas structureSchemas) where T : class
    	{
    		return new QueryBuilder<T>(structureSchemas, new ExpressionParsers());
    	}

    	public IExpressionParsers GetExpressionParsers()
		{
			return new ExpressionParsers();
		}
    }
}