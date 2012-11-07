using System;
using System.Data;
using System.Threading;
using SisoDb.Dac;
using SisoDb.Dac.BulkInserts;
using SisoDb.DbSchema;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.Querying.Sql;
using SisoDb.Structures;
using SisoDb.Structures.IdGenerators;
using SisoDb.Structures.Schemas;

namespace SisoDb.SqlServer
{
    public abstract class SqlServerProviderFactory : IDbProviderFactory
    {
        protected readonly Lazy<IConnectionManager> ConnectionManagerFn; 
	    protected readonly ISqlStatements SqlStatements;

        public StorageProviders ProviderType
        {
            get; private set;
        }

        public IConnectionManager ConnectionManager
        {
            get { return ConnectionManagerFn.Value; }
        }

        protected SqlServerProviderFactory(StorageProviders storageProvider, ISqlStatements sqlStatements)
        {
            ProviderType = storageProvider;
            SqlStatements = sqlStatements;
            ConnectionManagerFn = new Lazy<IConnectionManager>(CreateConnectionManager, LazyThreadSafetyMode.ExecutionAndPublication);
        }

	    protected virtual IConnectionManager CreateConnectionManager()
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
            return new SqlServerClient(GetAdoDriver(), connectionInfo, ConnectionManager, SqlStatements);
        }

        public virtual ITransactionalDbClient GetTransactionalDbClient(ISisoConnectionInfo connectionInfo)
        {
            var connection = ConnectionManager.OpenClientConnection(connectionInfo);
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
                Transactions.SuppressOngoingTransactionWhile(() => connection = ConnectionManager.OpenClientConnection(connectionInfo));
            else
                connection = ConnectionManager.OpenClientConnection(connectionInfo);

            return new SqlServerDbClient(
                GetAdoDriver(),
                connectionInfo,
                connection,
                null,
                ConnectionManager,
                SqlStatements);
	    }

        public virtual IDbSchemas GetDbSchemaManagerFor(ISisoDatabase db)
        {
            return new DbSchemas(db, new SqlDbSchemaUpserter(SqlStatements));
        }

        public virtual IStructureInserter GetStructureInserter(IDbClient dbClient)
        {
            return new DbStructureInserter(dbClient);
        }

        public virtual IStructureIdGenerator GetGuidStructureIdGenerator()
        {
            return new SequentialGuidStructureIdGenerator();
        }

        public virtual IIdentityStructureIdGenerator GetIdentityStructureIdGenerator(IDbClient dbClient)
    	{
            return new DbIdentityStructureIdGenerator(dbClient);
    	}

	    public virtual IQueryBuilder GetQueryBuilder(Type structureType, IStructureSchemas structureSchemas)
        {
            return new QueryBuilder(structureType, structureSchemas, new ExpressionParsers(structureSchemas.StructureSchemaBuilder.DataTypeConverter));
        }

	    public virtual IQueryBuilder<T> GetQueryBuilder<T>(IStructureSchemas structureSchemas) where T : class
        {
            return new QueryBuilder<T>(structureSchemas, new ExpressionParsers(structureSchemas.StructureSchemaBuilder.DataTypeConverter));
        }

	    public virtual ISqlExpressionBuilder GetSqlExpressionBuilder()
        {
            return new SqlExpressionBuilder(GetWhereCriteriaBuilder);
        }

        public virtual ISqlWhereCriteriaBuilder GetWhereCriteriaBuilder()
        {
            return new SqlServerWhereCriteriaBuilder();
        }

	    public abstract IDbQueryGenerator GetDbQueryGenerator();

	    public virtual INamedQueryGenerator<T> GetNamedQueryGenerator<T>(IStructureSchemas structureSchemas) where T : class
        {
            return new NamedQueryGenerator<T>(GetQueryBuilder<T>(structureSchemas), GetDbQueryGenerator(), new DbDataTypeTranslator());
        }
    }
}