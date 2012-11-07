using System.Data;
using SisoDb.Dac;
using SisoDb.Querying;
using SisoDb.Querying.Sql;
using SisoDb.SqlServer;

namespace SisoDb.Sql2005
{
    public class Sql2005ProviderFactory : SqlServerProviderFactory
    {
        public Sql2005ProviderFactory() 
            : base(StorageProviders.Sql2005, new Sql2005Statements()) { }

        public override IServerClient GetServerClient(ISisoConnectionInfo connectionInfo)
        {
            return new Sql2005ServerClient(GetAdoDriver(), connectionInfo, ConnectionManager, SqlStatements);
        }

        public override IDbClient GetTransactionalDbClient(ISisoConnectionInfo connectionInfo)
        {
            var connection = ConnectionManager.OpenClientConnection(connectionInfo);
            var transaction = Transactions.ActiveTransactionExists ? null : connection.BeginTransaction(IsolationLevel.ReadCommitted);

            return new Sql2005DbClient(
                GetAdoDriver(),
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

            return new Sql2005DbClient(
                GetAdoDriver(),
                connection,
                null,
                ConnectionManager,
                SqlStatements);
        }

        public override IAdoDriver GetAdoDriver()
        {
            return new Sql2005AdoDriver();
        }

        public override IDbQueryGenerator GetDbQueryGenerator()
        {
            return new Sql2005QueryGenerator(SqlStatements, GetSqlExpressionBuilder());
        }

        public override ISqlWhereCriteriaBuilder GetWhereCriteriaBuilder()
        {
            return new Sql2005WhereCriteriaBuilder();
        }
    }
}