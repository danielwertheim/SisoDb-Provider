namespace SisoDb.Sql2008
{
    public class Sql2008DbFactory : ISisoDbFactory
    {
        public virtual ISisoDatabase CreateDatabase(ISisoConnectionInfo connectionInfo, IDbProviderFactory providerFactory = null)
        {
            return new Sql2008Database(connectionInfo, providerFactory ?? new Sql2008ProviderFactory());
        }
    }
}