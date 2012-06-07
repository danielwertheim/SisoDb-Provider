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
using SisoDb.Structures;

namespace SisoDb.SqlServer
{
	public abstract class SqlServerProviderFactory : IDbProviderFactory
    {
		private IConnectionManager _connectionManager;
	    protected readonly ISqlStatements SqlStatements;

        public abstract StorageProviders ProviderType { get; }

        public IConnectionManager ConnectionManager
        {
            get { return _connectionManager; }
            set
            {
                Ensure.That(value, "ConnectionManager").IsNotNull();
                _connectionManager = value;
            }
        }

	    protected SqlServerProviderFactory(ISqlStatements sqlStatements)
        {
            _connectionManager = OnCreateConnectionManager();
	        SqlStatements = sqlStatements;
        }

	    protected virtual IConnectionManager OnCreateConnectionManager()
	    {
	        return new SqlServerConnectionManager(GetAdoDriver());
	    }

	    public virtual IAdoDriver GetAdoDriver()
	    {
	        return new SqlServerAdoDriver();
	    }

	    public virtual IDbSettings GetSettings()
        {
            return DbSettings.CreateDefault();
        }

        public virtual ISqlStatements GetSqlStatements()
		{
			return SqlStatements;
		}

		public virtual IServerClient GetServerClient(ISisoConnectionInfo connectionInfo)
        {
            return new DbServerClient(GetAdoDriver(), connectionInfo, _connectionManager, SqlStatements);
        }

        public virtual ITransactionalDbClient GetTransactionalDbClient(ISisoConnectionInfo connectionInfo)
        {
            var connection = ConnectionManager.OpenClientDbConnection(connectionInfo);
            var transaction = Transactions.ActiveTransactionExists ? null : connection.BeginTransaction(IsolationLevel.ReadCommitted);

            return new SqlServerDbClient(
                GetAdoDriver(),
                connectionInfo,
                connection,
                transaction,
                ConnectionManager,
                SqlStatements);
        }

        public virtual IDbClient GetNonTransactionalDbClient(ISisoConnectionInfo connectionInfo)
	    {
            IDbConnection connection = null;
            if (Transactions.ActiveTransactionExists)
                Transactions.SuppressOngoingTransactionWhile(() => connection = ConnectionManager.OpenClientDbConnection(connectionInfo));
            else
                connection = ConnectionManager.OpenClientDbConnection(connectionInfo);

            return new SqlServerDbClient(
                GetAdoDriver(),
                connectionInfo,
                connection,
                null,
                ConnectionManager,
                SqlStatements);
	    }

        public virtual IDbSchemaManager GetDbSchemaManagerFor(ISisoDatabase db)
        {
            return new DbSchemaManager(new SqlDbSchemaUpserter(db, SqlStatements));
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
            return new QueryBuilder(structureType, structureSchemas, new ExpressionParsers(structureSchemas.SchemaBuilder.DataTypeConverter));
        }

	    public virtual IQueryBuilder<T> GetQueryBuilder<T>(IStructureSchemas structureSchemas) where T : class
        {
            return new QueryBuilder<T>(structureSchemas, new ExpressionParsers(structureSchemas.SchemaBuilder.DataTypeConverter));
        }

	    public virtual ISqlExpressionBuilder GetSqlExpressionBuilder()
        {
            return new SqlExpressionBuilder(GetWhereCriteriaBuilder);
        }

        public virtual ISqlWhereCriteriaBuilder GetWhereCriteriaBuilder()
        {
            return new SqlWhereCriteriaBuilder();
        }

	    public abstract IDbQueryGenerator GetDbQueryGenerator();

	    public virtual INamedQueryGenerator<T> GetNamedQueryGenerator<T>(IStructureSchemas structureSchemas) where T : class
        {
            return new NamedQueryGenerator<T>(GetQueryBuilder<T>(structureSchemas), GetDbQueryGenerator(), new DbDataTypeTranslator());
        }
    }
}