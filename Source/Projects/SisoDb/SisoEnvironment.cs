using System;
using System.Linq;
using SisoDb.Providers;

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
        }
    }
}