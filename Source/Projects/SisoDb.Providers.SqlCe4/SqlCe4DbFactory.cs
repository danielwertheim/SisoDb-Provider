namespace SisoDb.SqlCe4
{
    public class SqlCe4DbFactory : ISisoDbFactory
    {
        static SqlCe4DbFactory()
        {
            SisoEnvironment.ProviderFactories.Register(StorageProviders.SqlCe4, new SqlCe4ProviderFactory());
        }

        public virtual ISisoDatabase CreateDatabase(ISisoConnectionInfo connectionInfo)
        {
            return new SqlCe4Database((SqlCe4ConnectionInfo)connectionInfo);
        }
    }
}