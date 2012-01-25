namespace SisoDb.Sql2012
{
    public class Sql2012DbFactory : ISisoDatabaseFactory
    {
		public static IDbProviderFactory ProviderFactory { get; set; }

    	static Sql2012DbFactory()
        {
			ProviderFactory = new Sql2012ProviderFactory();
        }

        public virtual ISisoDatabase CreateDatabase(ISisoConnectionInfo connectionInfo)
        {
            return new Sql2012Database(connectionInfo, ProviderFactory);
        }
    }
}