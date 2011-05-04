using SisoDb.Providers.DbSchema;
using SisoDb.Providers.SqlAzure.DbSchema;

namespace SisoDb.Providers.SqlAzure
{
    public class SqlAzureProviderFactory : ISisoProviderFactory
    {
        public IDbColumnGenerator GetDbColumnGenerator()
        {
            return new AzureDbColumnGenerator();
        }
    }
}