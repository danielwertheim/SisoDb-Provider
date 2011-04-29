using SisoDb.Providers.DbSchema;

namespace SisoDb.Providers
{
    public interface ISisoProviderFactory
    {
        IDbColumnGenerator GetDbColumnGenerator();
    }
}