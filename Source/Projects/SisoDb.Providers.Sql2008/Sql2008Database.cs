namespace SisoDb.Sql2008
{
    public class Sql2008Database : SisoDatabase
    {
        protected internal Sql2008Database(ISisoConnectionInfo connectionInfo) : base(connectionInfo)
        {
        }

		public override IReadSession CreateReadSession()
        {
            return new Sql2008ReadSession(
				this,
				ProviderFactory.GetNonTransactionalDbClient(ConnectionInfo),
				DbSchemaManager);
        }

        public override IUnitOfWork CreateUnitOfWork()
        {
            return new Sql2008UnitOfWork(
				this,
				ProviderFactory.GetTransactionalDbClient(ConnectionInfo),
				DbSchemaManager);
        }
    }
}