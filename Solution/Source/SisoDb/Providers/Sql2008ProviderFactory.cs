using SisoDb.Providers.DbSchema;
using SisoDb.Providers.SqlProvider.DbSchema;

namespace SisoDb.Providers
{
    public class Sql2008ProviderFactory : ISisoProviderFactory
    {
        public IDbColumnGenerator GetDbColumnGenerator()
        {
            return new SqlDbColumnGenerator();
        }
    }
}