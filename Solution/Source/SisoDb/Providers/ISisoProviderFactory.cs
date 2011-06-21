using SisoDb.DbSchema;

namespace SisoDb.Providers
{
    public interface ISisoProviderFactory
    {
        IDbColumnGenerator GetDbColumnGenerator();
    }
}