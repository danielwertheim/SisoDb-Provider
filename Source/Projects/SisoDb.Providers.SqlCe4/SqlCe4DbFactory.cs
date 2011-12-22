namespace SisoDb.SqlCe4
{
    public class SqlCe4DbFactory : ISisoDbFactory
    {
		public static IDbProviderFactory ProviderFactory { get; set; }

		static SqlCe4DbFactory()
        {
			ProviderFactory = new SqlCe4ProviderFactory();
        }

        public virtual ISisoDatabase CreateDatabase(ISisoConnectionInfo connectionInfo)
        {
			return new SqlCe4Database(connectionInfo, ProviderFactory);
        }
    }
}