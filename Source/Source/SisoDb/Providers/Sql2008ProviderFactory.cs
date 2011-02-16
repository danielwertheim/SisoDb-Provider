using SisoDb.Providers.Shared.DbSchema;
using SisoDb.Providers.SqlProvider.DbSchema;

namespace SisoDb.Providers
{
    public class Sql2008ProviderFactory : IProviderFactory
    {
        public IDbColumnGenerator GetDbColumnGenerator()
        {
            return new SqlDbColumnGenerator();
        }
    }
}