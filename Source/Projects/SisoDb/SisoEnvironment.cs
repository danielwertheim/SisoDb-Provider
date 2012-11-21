using System;
using SisoDb.NCore;

namespace SisoDb
{
    public static class SisoEnvironment
    {
        public static IFormatting Formatting;
        public static IStringConverter StringConverter;
        public static StringComparer StringComparer;

        static SisoEnvironment()
        {
            Formatting = new SisoDbFormatting();
            StringConverter = new StringConverter(Formatting);
            StringComparer = StringComparer.InvariantCultureIgnoreCase;
        }
    }
}