using SisoDb.Providers.DbSchema;
using SisoDb.Providers.SqlCe4.DbSchema;

namespace SisoDb.Providers.SqlCe4
{
    public class SqlCe4ProviderFactory : ISisoProviderFactory
    {
        public IDbColumnGenerator GetDbColumnGenerator()
        {
            return new SqlCe4DbColumnGenerator();
        }
    }
}