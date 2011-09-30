using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.SqlAzure.DbSchema;

namespace SisoDb.SqlAzure
{
    public class SqlAzureProviderFactory : ISisoProviderFactory
    {
        public IDbColumnGenerator GetDbColumnGenerator()
        {
            return new AzureDbColumnGenerator();
        }
    }
}