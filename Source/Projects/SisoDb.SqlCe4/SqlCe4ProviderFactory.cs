using System;
using System.Data;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.PineCone.Structures.Schemas;
using SisoDb.Querying;
using SisoDb.Querying.Sql;
using SisoDb.SqlServer;

namespace SisoDb.SqlCe4
{
    public class SqlCe4ProviderFactory : SqlServerProviderFactory
    {
        public SqlCe4ProviderFactory() 
            : base(StorageProviders.SqlCe4, new SqlCe4Statements()) { }

        protected override IConnectionManager CreateConnectionManager()
        {
            return new SqlCe4ConnectionManager(GetAdoDriver());
        }

        public override IAdoDriver GetAdoDriver()
	    {
            return new SqlCe4AdoDriver();
	    }

		public override IServerClient GetServerClient(ISisoConnectionInfo connectionInfo)
        {
            return new SqlCe4ServerClient(GetAdoDriver(),(SqlCe4ConnectionInfo)connectionInfo, ConnectionManager, SqlStatements);
        }

        public override ITransactionalDbClient GetTransactionalDbClient(ISisoConnectionInfo connectionInfo)
        {
            var connection = ConnectionManager.OpenClientConnection(connectionInfo);
            var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

            return new SqlCe4DbClient(
                GetAdoDriver(),
                connectionInfo,
                connection,
                transaction,
                ConnectionManager, 
                SqlStatements);
        }

        public override IDbClient GetNonTransactionalDbClient(ISisoConnectionInfo connectionInfo)
	    {
            IDbConnection connection = null;
            if (Transactions.ActiveTransactionExists)
                Transactions.SuppressOngoingTransactionWhile(() => connection = ConnectionManager.OpenClientConnection(connectionInfo));
            else
                connection = ConnectionManager.OpenClientConnection(connectionInfo);

            return new SqlCe4DbClient(
                GetAdoDriver(), 
                connectionInfo,
                connection,
                null,
                ConnectionManager,
                SqlStatements);
	    }

        public override IDbQueryGenerator GetDbQueryGenerator()
        {
            return new SqlCe4QueryGenerator(SqlStatements, GetSqlExpressionBuilder());
        }

        public override ISqlWhereCriteriaBuilder GetWhereCriteriaBuilder()
        {
            return new SqlCe4WhereCriteriaBuilder();
        }

	    public override INamedQueryGenerator<T> GetNamedQueryGenerator<T>(IStructureSchemas structureSchemas)
	    {
	        throw new NotSupportedException("SQL CE4 does not support Stored procedures.");
	    }
    }
}