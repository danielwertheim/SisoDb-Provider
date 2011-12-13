namespace SisoDb.SqlCe4
{
    public class SqlCe4Database : SisoDatabase
    {
        protected internal SqlCe4Database(ISisoConnectionInfo connectionInfo)
            : base(connectionInfo)
        {
        }

		public override IReadSession CreateReadSession()
        {
            return new SqlCe4ReadSession(
				this,
				ProviderFactory.GetNonTransactionalDbClient(ConnectionInfo), //TODO: Why IReadSession as IDisposable? If not we could use DB.DbNonTransClient
				DbSchemaManager);
        }

        public override IUnitOfWork CreateUnitOfWork()
        {
            return new SqlCe4UnitOfWork(
				this,
				ProviderFactory.GetTransactionalDbClient(ConnectionInfo),
				DbSchemaManager);
        }
    }
}