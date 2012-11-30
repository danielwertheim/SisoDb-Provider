using System;
using System.Data;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Querying;
using SisoDb.Querying.Sql;
using SisoDb.SqlServer;
using SisoDb.Structures.Schemas;

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

        public override IServerClient GetServerClient(ISisoDatabase db)
        {
            return new SqlCe4ServerClient(GetAdoDriver(),(SqlCe4ConnectionInfo)db.ConnectionInfo, ConnectionManager, SqlStatements);
        }

        public override IDbClient GetTransactionalDbClient(ISisoDatabase db)
        {
            var connection = ConnectionManager.OpenClientConnection(db.ConnectionInfo);
            var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

            return new SqlCe4DbClient(
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

            return new SqlCe4DbClient(
                GetAdoDriver(), 
                connection,
                ConnectionManager,
                SqlStatements,
                db.Pipe);
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