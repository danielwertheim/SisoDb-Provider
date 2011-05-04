using SisoDb.Providers.DbSchema;
using SisoDb.Providers.SqlCe4Provider.DbSchema;

namespace SisoDb.Providers
{
    public class SqlCe4ProviderFactory : ISisoProviderFactory
    {
        public IDbColumnGenerator GetDbColumnGenerator()
        {
            return new SqlCe4DbColumnGenerator();
        }
    }
}