using System;
using SisoDb.NCore;
using SisoDb.NCore.Cryptography;

namespace SisoDb
{
    public static class SisoEnvironment
    {
        public static readonly IFormatting Formatting;

        public static readonly IStringConverter StringConverter;

        public static readonly StringComparer StringComparer;

        public static readonly IHashService HashService;

        static SisoEnvironment()
        {
            Formatting = new SisoDbFormatting();
            StringConverter = new StringConverter(Formatting);
            StringComparer = StringComparer.InvariantCultureIgnoreCase;
            HashService = new Crc32HashService();
        }
    }
}