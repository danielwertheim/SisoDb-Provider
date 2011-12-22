namespace SisoDb.Sql2008
{
    public class Sql2008DbFactory : ISisoDbFactory
    {
		public static IDbProviderFactory ProviderFactory { get; set; }

    	static Sql2008DbFactory()
        {
			ProviderFactory = new Sql2008ProviderFactory();
        }

        public virtual ISisoDatabase CreateDatabase(ISisoConnectionInfo connectionInfo)
        {
            return new Sql2008Database(connectionInfo, ProviderFactory);
        }
    }
}