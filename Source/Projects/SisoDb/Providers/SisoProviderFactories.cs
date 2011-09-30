using System.Collections.Generic;

namespace SisoDb.Providers
{
    public class SisoProviderFactories : ISisoProviderFactories
    {
        private readonly Dictionary<StorageProviders, ISisoProviderFactory> _providerFactories;

        public SisoProviderFactories()
        {
            _providerFactories = new Dictionary<StorageProviders, ISisoProviderFactory>();
        }

        public ISisoProviderFactory Get(StorageProviders storageProvider)
        {
            return _providerFactories[storageProvider];
        }

        public void Register(StorageProviders storageProvider, ISisoProviderFactory providerFactory)
        {
            _providerFactories.Add(storageProvider, providerFactory);
        }

        public void Replace(StorageProviders storageProvider, ISisoProviderFactory providerFactory)
        {
            _providerFactories.Remove(storageProvider);
            
            Register(storageProvider, providerFactory);
        }
    }
}