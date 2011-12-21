namespace SisoDb.SqlCe4
{
    public class SqlCe4Database : DbDatabase
    {
        protected internal SqlCe4Database(ISisoConnectionInfo connectionInfo, ISisoProviderFactory providerFactory)
            : base(connectionInfo, providerFactory)
        {
        }

		public override IQueryEngine CreateQueryEngine()
        {
            return new SqlCe4QueryEngine(
				this,
				ProviderFactory.GetNonTransactionalDbClient(ConnectionInfo),
				DbSchemaManager,
				SqlCe4Statements.Instance);
        }

        public override IUnitOfWork CreateUnitOfWork()
        {
			var dbClient = ProviderFactory.GetTransactionalDbClient(ConnectionInfo);
			var dbClientNonTransactional = ProviderFactory.GetNonTransactionalDbClient(ConnectionInfo);

			return new SqlCe4UnitOfWork(
				this,
				dbClient,
				dbClientNonTransactional,
				DbSchemaManager,
				ProviderFactory.GetIdentityStructureIdGenerator(dbClientNonTransactional),
				SqlCe4Statements.Instance);
        }
    }
}