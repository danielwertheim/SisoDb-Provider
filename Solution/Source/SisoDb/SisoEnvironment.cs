using System;
using System.Linq;
using SisoDb.Providers;
using SisoDb.Providers.Sql2008;

namespace SisoDb
{
    public static class SisoEnvironment
    {
        public static readonly ISisoDbFormatting Formatting;

        public static readonly ResourceContainer Resources;

        public static readonly ISisoProviderFactories ProviderFactories;

        static SisoEnvironment()
        {
            Formatting = new SisoDbFormatting();
            Resources = new ResourceContainer();

            ProviderFactories = new SisoProviderFactories();
            ProviderFactories.Register(StorageProviders.Sql2008, new Sql2008ProviderFactory());
        }
    }
}