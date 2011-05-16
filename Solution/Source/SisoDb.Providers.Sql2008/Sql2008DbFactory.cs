namespace SisoDb.Providers.Sql2008
{
    public class Sql2008DbFactory : ISisoDbFactory
    {
        static Sql2008DbFactory()
        {
            SisoEnvironment.ProviderFactories.Register(StorageProviders.Sql2008, new Sql2008ProviderFactory());
        }

        public ISisoDatabase CreateDatabase(ISisoConnectionInfo connectionInfo)
        {
            return new SqlDatabase(connectionInfo);
        }
    }
}