using SisoDb.Providers.DbSchema;
using SisoDb.Providers.Sql2008.DbSchema;

namespace SisoDb.Providers.Sql2008
{
    public class Sql2008ProviderFactory : ISisoProviderFactory
    {
        public IDbColumnGenerator GetDbColumnGenerator()
        {
            return new Sql2008DbColumnGenerator();
        }
    }
}