namespace SisoDb.SqlCe4
{
    public class SqlCe4Database : SisoDbDatabase
    {
        protected internal SqlCe4Database(ISisoConnectionInfo connectionInfo, IDbProviderFactory dbProviderFactory)
            : base(connectionInfo, dbProviderFactory)
        {
        }

		protected override DbReadSession CreateReadSession()
        {
            return new SqlCe4ReadSession(
				this,
				ProviderFactory.GetNonTransactionalDbClient(ConnectionInfo));
        }

        protected override DbWriteSession CreateWriteSession()
        {
			var dbClient = ProviderFactory.GetTransactionalDbClient(ConnectionInfo);
			var dbClientNonTransactional = ProviderFactory.GetNonTransactionalDbClient(ConnectionInfo);

			return new SqlCe4WriteSession(
				this,
				dbClient,
				dbClientNonTransactional,
				ProviderFactory.GetIdentityStructureIdGenerator(dbClientNonTransactional));
        }
    }
}