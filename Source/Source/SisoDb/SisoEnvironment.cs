using SisoDb.Providers;

namespace SisoDb
{
    public static class SisoEnvironment
    {
        public static readonly ISisoDbFormatting Formatting;

        public static readonly ResourceContainer Resources;

        public static readonly SisoProviderFactories ProviderFactories;
        
        static SisoEnvironment()
        {
            Formatting = new SisoDbFormatting();
            Resources = new ResourceContainer();

            ProviderFactories = new SisoProviderFactories();
            ProviderFactories.Register(StorageProviders.SqlAzure, new SqlAzureProviderFactory());
            ProviderFactories.Register(StorageProviders.Sql2008, new Sql2008ProviderFactory());
        }
    }
}