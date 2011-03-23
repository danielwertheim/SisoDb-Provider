using SisoDb.Providers.DbSchema;

namespace SisoDb.Providers
{
    public interface IProviderFactory
    {
        IDbColumnGenerator GetDbColumnGenerator();
    }
}