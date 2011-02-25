using SisoDb.Providers.Shared.DbSchema;
using SisoDb.Providers.SqlProvider;

namespace SisoDb.Providers
{
    public interface IProviderFactory
    {
        IDbColumnGenerator GetDbColumnGenerator();
    }
}