using SisoDb.Providers.Shared.DbSchema;

namespace SisoDb.Providers
{
    public interface IProviderFactory
    {
        IDbColumnGenerator GetDbColumnGenerator();
    }
}