namespace SisoDb.SqlCe4
{
    public class SqlCe4Database : DbDatabase
    {
        protected internal SqlCe4Database(ISisoConnectionInfo connectionInfo)
            : base(connectionInfo)
        {
        }

		public override IReadSession CreateReadSession()
        {
            return new SqlCe4ReadSession(
				this,
				ProviderFactory.GetNonTransactionalDbClient(ConnectionInfo),
				DbSchemaManager);
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
				ProviderFactory.GetIdentityStructureIdGenerator(dbClientNonTransactional));
        }
    }
}