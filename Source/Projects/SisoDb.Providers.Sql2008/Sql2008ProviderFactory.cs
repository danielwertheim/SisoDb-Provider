using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.Sql2008.DbSchema;

namespace SisoDb.Sql2008
{
    public class Sql2008ProviderFactory : ISisoProviderFactory
    {
        public IDbColumnGenerator GetDbColumnGenerator()
        {
            return new SqlDbColumnGenerator();
        }
    }
}