namespace SisoDb.Providers
{
    public interface ISisoProviderFactories
    {
        ISisoProviderFactory Get(StorageProviders storageProvider);
        void Register(StorageProviders storageProvider, ISisoProviderFactory providerFactory);
        void Replace(StorageProviders storageProvider, ISisoProviderFactory providerFactory);
    }
}