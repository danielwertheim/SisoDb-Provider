namespace SisoDb.Sql2012
{
    public class Sql2012Database : SisoDbDatabase
    {
    	public Sql2012Database(ISisoConnectionInfo connectionInfo, IDbProviderFactory dbProviderFactory) 
			: base(connectionInfo, dbProviderFactory)
        {
        }

    	protected override DbReadSession CreateReadSession()
        {
            return new Sql2012ReadSession(
				this,
				ProviderFactory.GetNonTransactionalDbClient(ConnectionInfo));
        }

        protected override DbWriteSession CreateWriteSession()
        {
        	var dbClient = ProviderFactory.GetTransactionalDbClient(ConnectionInfo);
			var dbClientNonTransactional = ProviderFactory.GetNonTransactionalDbClient(ConnectionInfo);

			return new Sql2012WriteSession(
				this,
				dbClient,
				dbClientNonTransactional,
				ProviderFactory.GetIdentityStructureIdGenerator(dbClientNonTransactional));
        }
    }
}