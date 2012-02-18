using System.Data;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Dac.BulkInserts;
using SisoDb.DbSchema;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.Sql2012.Dac;
using SisoDb.Structures;

namespace SisoDb.Sql2012
{
	public class Sql2012ProviderFactory : IDbProviderFactory
    {
		private readonly IConnectionManager _connectionManager;
        private readonly ISqlStatements _sqlStatements;
		
        public Sql2012ProviderFactory()
        {
			_connectionManager = new Sql2012ConnectionManager();
            _sqlStatements = new Sql2012Statements();
        }

        public StorageProviders ProviderType
        {
            get { return StorageProviders.Sql2012; }
        }

        public IConnectionManager ConnectionManager
        {
            get { return _connectionManager; }
        }

		public ISqlStatements GetSqlStatements()
		{
			return _sqlStatements;
		}

		public virtual IServerClient GetServerClient(ISisoConnectionInfo connectionInfo)
        {
            return new Sql2012ServerClient(connectionInfo, _connectionManager, _sqlStatements);
        }

        public ITransactionalDbClient GetTransactionalDbClient(ISisoConnectionInfo connectionInfo)
        {
            var connection = _connectionManager.OpenClientDbConnection(connectionInfo);
            var transaction = Transactions.ActiveTransactionExists ? null : connection.BeginTransaction(IsolationLevel.ReadCommitted);

            return new Sql2012DbClient(
                connectionInfo,
                connection,
                transaction,
                _connectionManager,
                _sqlStatements);
        }

	    public IDbClient GetNonTransactionalDbClient(ISisoConnectionInfo connectionInfo)
	    {
            IDbConnection connection = null;
            if (Transactions.ActiveTransactionExists)
                Transactions.SuppressOngoingTransactionWhile(() => connection = _connectionManager.OpenClientDbConnection(connectionInfo));
            else
                connection = _connectionManager.OpenClientDbConnection(connectionInfo);

            return new Sql2012DbClient(
                connectionInfo,
                connection,
                null,
                _connectionManager,
                _sqlStatements);
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
            return new Sql2012QueryGenerator(_sqlStatements);
        }

    	public IQueryBuilder<T> GetQueryBuilder<T>(IStructureSchemas structureSchemas) where T : class
    	{
    		return new QueryBuilder<T>(structureSchemas, new ExpressionParsers());
    	}
    }
}