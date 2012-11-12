using System;
using SisoDb.NCore;
using SisoDb.NCore.Cryptography;

namespace SisoDb
{
    public static class SisoEnvironment
    {
        public static IFormatting Formatting;
        public static IStringConverter StringConverter;
        public static StringComparer StringComparer;
        public static IHashService HashService;

        static SisoEnvironment()
        {
            Formatting = new SisoDbFormatting();
            StringConverter = new StringConverter(Formatting);
            StringComparer = StringComparer.InvariantCultureIgnoreCase;
            HashService = new Crc32HashService();
        }
    }
}