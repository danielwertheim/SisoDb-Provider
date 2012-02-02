namespace SisoDb.Sql2008
{
    public class Sql2008Database : SisoDbDatabase
    {
        public Sql2008Database(ISisoConnectionInfo connectionInfo, IDbProviderFactory dbProviderFactory) 
			: base(connectionInfo, dbProviderFactory)
        {
        }

    	protected override DbReadSession CreateReadSession()
        {
            return new Sql2008ReadSession(
				this,
				ProviderFactory.GetNonTransactionalDbClient(ConnectionInfo));
        }

        protected override DbWriteSession CreateWriteSession()
        {
        	var dbClient = ProviderFactory.GetTransactionalDbClient(ConnectionInfo);
			var dbClientNonTransactional = ProviderFactory.GetNonTransactionalDbClient(ConnectionInfo);

			return new Sql2008WriteSession(
				this,
				dbClient,
				dbClientNonTransactional,
				ProviderFactory.GetIdentityStructureIdGenerator(dbClientNonTransactional));
        }
    }
}