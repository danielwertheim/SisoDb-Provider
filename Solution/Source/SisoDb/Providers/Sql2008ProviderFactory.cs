using SisoDb.Providers.DbSchema;
using SisoDb.Providers.Sql2008Provider.DbSchema;

namespace SisoDb.Providers
{
    public class Sql2008ProviderFactory : ISisoProviderFactory
    {
        public IDbColumnGenerator GetDbColumnGenerator()
        {
            return new Sql2008DbColumnGenerator();
        }
    }
}