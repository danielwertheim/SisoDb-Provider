namespace SisoDb.SqlAzure
{
    public class SqlAzureDbFactory : ISisoDbFactory
    {
        static SqlAzureDbFactory()
        {
            SisoEnvironment.ProviderFactories.Register(StorageProviders.SqlCe4, new SqlAzureProviderFactory());
        }

        public ISisoDatabase CreateDatabase(ISisoConnectionInfo connectionInfo)
        {
            return new SqlAzureDatabase(connectionInfo);
        }
    }
}