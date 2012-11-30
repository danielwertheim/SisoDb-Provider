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

        public override IServerClient GetServerClient(ISisoDatabase db)
        {
            return new Sql2005ServerClient(GetAdoDriver(), db.ConnectionInfo, ConnectionManager, SqlStatements);
        }

        public override IDbClient GetTransactionalDbClient(ISisoDatabase db)
        {
            var connection = ConnectionManager.OpenClientConnection(db.ConnectionInfo);
            var transaction = Transactions.ActiveTransactionExists ? null : connection.BeginTransaction(IsolationLevel.ReadCommitted);

            return new Sql2005DbClient(
                GetAdoDriver(),
                connection,
                transaction,
                ConnectionManager,
                SqlStatements,
                db.Pipe);
        }

        public override IDbClient GetNonTransactionalDbClient(ISisoDatabase db)
        {
            IDbConnection connection = null;
            if (Transactions.ActiveTransactionExists)
                Transactions.SuppressOngoingTransactionWhile(() => connection = ConnectionManager.OpenClientConnection(db.ConnectionInfo));
            else
                connection = ConnectionManager.OpenClientConnection(db.ConnectionInfo);

            return new Sql2005DbClient(
                GetAdoDriver(),
                connection,
                null,
                ConnectionManager,
                SqlStatements,
                db.Pipe);
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