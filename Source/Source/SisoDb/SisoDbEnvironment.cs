using System.Collections.Generic;
using SisoDb.Providers;

namespace SisoDb
{
    public static class SisoDbEnvironment
    {
        private static readonly Dictionary<StorageProviders, IProviderFactory> ProviderFactories;

        public static readonly ResourceContainer ResourceContainer;

        public static readonly ISisoDbFormatting Formatting;
        
        static SisoDbEnvironment()
        {
            Formatting = new SisoDbFormatting();
            ResourceContainer = new ResourceContainer();
            
            ProviderFactories = new Dictionary<StorageProviders, IProviderFactory>();
            RegisterProviderFactory(StorageProviders.SqlAzure, new SqlAzureProviderFactory());
            RegisterProviderFactory(StorageProviders.Sql2008, new Sql2008ProviderFactory());
        }

        public static IProviderFactory GetProviderFactory(StorageProviders storageProvider)
        {
            return ProviderFactories[storageProvider];
        }

        public static void RegisterProviderFactory(StorageProviders storageProvider, IProviderFactory providerFactory)
        {
            ProviderFactories.Add(storageProvider, providerFactory);
        }

        public static void ReplaceProviderFactory(StorageProviders storageProvider, IProviderFactory providerFactory)
        {
            ProviderFactories.Remove(storageProvider);
            RegisterProviderFactory(storageProvider, providerFactory);
        }
    }
}