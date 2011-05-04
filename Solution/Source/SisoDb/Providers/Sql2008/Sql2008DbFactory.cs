namespace SisoDb.Providers.Sql2008
{
    public class Sql2008DbFactory : ISisoDbFactory
    {
        public ISisoDatabase CreateDatabase(ISisoConnectionInfo connectionInfo)
        {
            return new Sql2008Database(connectionInfo);
        }
    }
}