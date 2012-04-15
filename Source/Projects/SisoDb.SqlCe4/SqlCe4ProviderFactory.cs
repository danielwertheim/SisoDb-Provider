using System;
using System.Data;
using EnsureThat;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Dac.BulkInserts;
using SisoDb.DbSchema;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.Querying.Sql;
using SisoDb.SqlCe4.Dac;
using SisoDb.Structures;

namespace SisoDb.SqlCe4
{
	public class SqlCe4ProviderFactory : IDbProviderFactory
    {
		private IConnectionManager _connectionManager;
        private readonly ISqlStatements _sqlStatements;

        public SqlCe4ProviderFactory()
        {
            _connectionManager = new SqlCe4ConnectionManager(GetAdoDriver());
            _sqlStatements = new SqlCe4Statements();
        }

        public StorageProviders ProviderType
        {
            get { return StorageProviders.SqlCe4; }
        }

        public IConnectionManager ConnectionManager
        {
            get { return _connectionManager; }
            set
            {
                Ensure.That(value, "ConnectionManager").IsNotNull();
                _connectionManager = value;
            }
        }

	    public virtual IAdoDriver GetAdoDriver()
	    {
            return new SqlCe4AdoDriver();
	    }

	    public virtual IDbSettings GetSettings()
        {
            return DbSettings.CreateDefault();
        }

	    public virtual ISqlStatements GetSqlStatements()
		{
			return _sqlStatements;
		}

		public virtual IServerClient GetServerClient(ISisoConnectionInfo connectionInfo)
        {
            return new SqlCe4ServerClient(GetAdoDriver(),(SqlCe4ConnectionInfo)connectionInfo, _connectionManager, _sqlStatements);
        }

        public virtual ITransactionalDbClient GetTransactionalDbClient(ISisoConnectionInfo connectionInfo)
        {
            var connection = _connectionManager.OpenClientDbConnection(connectionInfo);
            var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

            return new SqlCe4DbClient(
                GetAdoDriver(),
                connectionInfo,
                connection,
                transaction,
                _connectionManager, 
                _sqlStatements);
        }

        public virtual IDbClient GetNonTransactionalDbClient(ISisoConnectionInfo connectionInfo)
	    {
            IDbConnection connection = null;
            if (Transactions.ActiveTransactionExists)
                Transactions.SuppressOngoingTransactionWhile(() => connection = _connectionManager.OpenClientDbConnection(connectionInfo));
            else
                connection = _connectionManager.OpenClientDbConnection(connectionInfo);

            return new SqlCe4DbClient(
                GetAdoDriver(), 
                connectionInfo,
                connection,
                null,
                _connectionManager,
                _sqlStatements);
	    }

	    public virtual IDbSchemaManager GetDbSchemaManagerFor(ISisoDatabase db)
        {
			return new DbSchemaManager(new SqlDbSchemaUpserter(db, _sqlStatements));
        }

        public virtual IStructureInserter GetStructureInserter(IDbClient dbClient)
        {
            return new DbStructureInserter(dbClient);
        }

        public virtual IIdentityStructureIdGenerator GetIdentityStructureIdGenerator(CheckOutAngGetNextIdentity action)
    	{
    		return new DbIdentityStructureIdGenerator(action);
    	}

    	public virtual IQueryBuilder GetQueryBuilder(Type structureType, IStructureSchemas structureSchemas)
        {
            return new QueryBuilder(structureType, structureSchemas, new ExpressionParsers());
        }

        public virtual IQueryBuilder<T> GetQueryBuilder<T>(IStructureSchemas structureSchemas) where T : class
        {
            return new QueryBuilder<T>(structureSchemas, new ExpressionParsers());
        }

        public virtual ISqlExpressionBuilder GetSqlExpressionBuilder()
        {
            return new SqlExpressionBuilder();
        }

        public virtual IDbQueryGenerator GetDbQueryGenerator()
        {
            return new SqlCe4QueryGenerator(_sqlStatements, GetSqlExpressionBuilder());
        }

	    public INamedQueryGenerator<T> GetNamedQueryGenerator<T>(IStructureSchemas structureSchemas) where T : class
	    {
	        throw new NotSupportedException("SQL CE4 does not support Stored procedures.");
	    }
    }
}