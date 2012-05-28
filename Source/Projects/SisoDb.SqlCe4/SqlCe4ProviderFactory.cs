using System;
using System.Data;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Querying;
using SisoDb.SqlServer;

namespace SisoDb.SqlCe4
{
    public class SqlCe4ProviderFactory : SqlServerProviderFactory
    {
        public override StorageProviders ProviderType
        {
            get { return StorageProviders.SqlCe4; }
        }

        public SqlCe4ProviderFactory() : base(new SqlCe4Statements())
        { }

        protected override IConnectionManager OnCreateConnectionManager()
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
            var connection = ConnectionManager.OpenClientDbConnection(connectionInfo);
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
                Transactions.SuppressOngoingTransactionWhile(() => connection = ConnectionManager.OpenClientDbConnection(connectionInfo));
            else
                connection = ConnectionManager.OpenClientDbConnection(connectionInfo);

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

	    public override INamedQueryGenerator<T> GetNamedQueryGenerator<T>(IStructureSchemas structureSchemas)
	    {
	        throw new NotSupportedException("SQL CE4 does not support Stored procedures.");
	    }
    }
}