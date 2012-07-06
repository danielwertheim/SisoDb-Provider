namespace SisoDb.SqlCe4
{
    public class SqlCe4DbFactory : ISisoDbFactory
    {
        public virtual ISisoDatabase CreateDatabase(ISisoConnectionInfo connectionInfo, IDbProviderFactory providerFactory = null)
        {
            return new SqlCe4Database(connectionInfo, providerFactory ?? new SqlCe4ProviderFactory());
        }
    }
}