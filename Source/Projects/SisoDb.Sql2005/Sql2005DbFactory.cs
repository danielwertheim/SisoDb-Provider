namespace SisoDb.Sql2005
{
    public class Sql2005DbFactory : ISisoDatabaseFactory
    {
		public static IDbProviderFactory ProviderFactory { get; set; }

    	static Sql2005DbFactory()
        {
			ProviderFactory = new Sql2005ProviderFactory();
        }

        public virtual ISisoDatabase CreateDatabase(ISisoConnectionInfo connectionInfo)
        {
            return new Sql2005Database(connectionInfo, ProviderFactory);
        }
    }
}