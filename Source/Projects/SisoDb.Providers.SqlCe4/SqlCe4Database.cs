namespace SisoDb.SqlCe4
{
    public class SqlCe4Database : DbDatabase
    {
        protected internal SqlCe4Database(ISisoConnectionInfo connectionInfo, IDbProviderFactory dbProviderFactory)
            : base(connectionInfo, dbProviderFactory)
        {
        }

		public override IQueryEngine CreateQueryEngine()
        {
            return new SqlCe4QueryEngine(
				this,
				ProviderFactory.GetNonTransactionalDbClient(ConnectionInfo));
        }

        public override IUnitOfWork CreateUnitOfWork()
        {
			var dbClient = ProviderFactory.GetTransactionalDbClient(ConnectionInfo);
			var dbClientNonTransactional = ProviderFactory.GetNonTransactionalDbClient(ConnectionInfo);

			return new SqlCe4UnitOfWork(
				this,
				dbClient,
				dbClientNonTransactional,
				ProviderFactory.GetIdentityStructureIdGenerator(dbClientNonTransactional));
        }
    }
}