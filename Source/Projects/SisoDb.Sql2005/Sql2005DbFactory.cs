namespace SisoDb.Sql2005
{
    public class Sql2005DbFactory : ISisoDbFactory
    {
        public virtual ISisoDatabase CreateDatabase(ISisoConnectionInfo connectionInfo, IDbProviderFactory providerFactory = null)
        {
            return new Sql2005Database(connectionInfo, providerFactory ?? new Sql2005ProviderFactory());
        }
    }
}