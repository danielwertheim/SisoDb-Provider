namespace SisoDb.Azure
{
    public class SqlAzureDbFactory : ISisoDbFactory
    {
        public virtual ISisoDatabase CreateDatabase(ISisoConnectionInfo connectionInfo, IDbProviderFactory providerFactory = null)
        {
            return new SqlAzureDatabase(connectionInfo, providerFactory ?? new SqlAzureProviderFactory());
        }
    }
}