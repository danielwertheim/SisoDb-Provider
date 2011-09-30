using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.SqlCe4.DbSchema;

namespace SisoDb.SqlCe4
{
    public class SqlCe4ProviderFactory : ISisoProviderFactory
    {
        public IDbColumnGenerator GetDbColumnGenerator()
        {
            return new SqlCe4DbColumnGenerator();
        }
    }
}