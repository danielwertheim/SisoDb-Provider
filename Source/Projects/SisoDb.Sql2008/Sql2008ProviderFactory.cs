using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Dac.BulkInserts;
using SisoDb.DbSchema;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.Sql2008.Dac;
using SisoDb.Structures;

namespace SisoDb.Sql2008
{
	public class Sql2008ProviderFactory : IDbProviderFactory
    {
		private readonly IConnectionManager _connectionManager;
        private readonly ISqlStatements _sqlStatements;
		
        public Sql2008ProviderFactory()
        {
			_connectionManager = new Sql2008ConnectionManager();
            _sqlStatements = new Sql2008Statements();
        }

        public StorageProviders ProviderType
        {
            get { return StorageProviders.Sql2008; }
        }

		public ISqlStatements GetSqlStatements()
		{
			return _sqlStatements;
		}

		public virtual IServerClient GetServerClient(ISisoConnectionInfo connectionInfo)
        {
            return new Sql2008ServerClient(connectionInfo, _connectionManager, _sqlStatements);
        }

        public IDbClient GetDbClient(ISisoConnectionInfo connectionInfo)
        {
            return new Sql2008DbClient(connectionInfo, _connectionManager, _sqlStatements);
        }

        public virtual IDbSchemaManager GetDbSchemaManager()
        {
			return new DbSchemaManager(new SqlDbSchemaUpserter(_sqlStatements));
        }

        public virtual IStructureInserter GetStructureInserter(IDbClient dbClient)
        {
            return new DbStructureInserter(dbClient);
        }

    	public IIdentityStructureIdGenerator GetIdentityStructureIdGenerator(CheckOutAngGetNextIdentity action)
    	{
    		return new DbIdentityStructureIdGenerator(action);
    	}

    	public virtual IDbQueryGenerator GetDbQueryGenerator()
        {
            return new Sql2008QueryGenerator(_sqlStatements);
        }

    	public IQueryBuilder<T> GetQueryBuilder<T>(IStructureSchemas structureSchemas) where T : class
    	{
    		return new QueryBuilder<T>(structureSchemas, new ExpressionParsers());
    	}
    }
}