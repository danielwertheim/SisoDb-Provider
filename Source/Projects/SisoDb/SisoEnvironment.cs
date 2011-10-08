using System;
using NCore;
using SisoDb.Providers;

namespace SisoDb
{
    public static class SisoEnvironment
    {
        public static readonly IFormatting Formatting;

        public static readonly IStringConverter StringConverter;

        public static readonly StringComparer StringComparer;

        public static readonly ResourceContainer Resources;

        public static readonly ISisoProviderFactories ProviderFactories;

        static SisoEnvironment()
        {
            Formatting = new SisoDbFormatting();
            StringConverter = new StringConverter(Formatting);
            StringComparer = StringComparer.InvariantCultureIgnoreCase;
            Resources = new ResourceContainer();
            ProviderFactories = new SisoProviderFactories();
        }
    }
}