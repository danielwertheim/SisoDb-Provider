using SisoDb.Providers.AzureProvider;
using SisoDb.Providers.SqlProvider;
using SisoDb.Resources;

namespace SisoDb
{
    public class SisoDbFactory : ISisoDbFactory
    {
        public ISisoDatabase CreateDatabase(ISisoConnectionInfo connectionInfo)
        {
            switch (connectionInfo.ProviderType)
            {
                case StorageProviders.Sql2008:
                    return new SqlDatabase(connectionInfo);
                case StorageProviders.SqlAzure:
                    return new AzureDatabase(connectionInfo);
                default:
                    throw new SisoDbException(
                        ExceptionMessages.SisoDbFactory_UnknownStorageProvider.Inject(connectionInfo.ProviderType));
            }
        }
    }
}