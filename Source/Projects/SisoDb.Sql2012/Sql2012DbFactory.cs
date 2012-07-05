namespace SisoDb.Sql2012
{
    public class Sql2012DbFactory : ISisoDbFactory
    {
        public virtual ISisoDatabase CreateDatabase(ISisoConnectionInfo connectionInfo, IDbProviderFactory providerFactory = null)
        {
            return new Sql2012Database(connectionInfo, providerFactory ?? new Sql2012ProviderFactory());
        }
    }
}