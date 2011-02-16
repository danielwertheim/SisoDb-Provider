using SisoDb.Providers.AzureProvider.DbSchema;
using SisoDb.Providers.Shared.DbSchema;

namespace SisoDb.Providers
{
    public class SqlAzureProviderFactory : IProviderFactory
    {
        public IDbColumnGenerator GetDbColumnGenerator()
        {
            return new AzureDbColumnGenerator();
        }
    }
}