using SisoDb.Core;
using SisoDb.Providers.AzureProvider;
using SisoDb.Providers.Sql2008Provider;
using SisoDb.Providers.SqlCe4Provider;
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
                    return new Sql2008Database(connectionInfo);
                case StorageProviders.SqlAzure:
                    return new AzureDatabase(connectionInfo);
                case StorageProviders.SqlCe4:
                    return new SqlCe4Database(connectionInfo);
                default:
                    throw new SisoDbException(
                        ExceptionMessages.SisoDbFactory_UnknownStorageProvider.Inject(connectionInfo.ProviderType));
            }
        }
    }
}