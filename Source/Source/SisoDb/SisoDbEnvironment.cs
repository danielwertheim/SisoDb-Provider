using System;
using System.Globalization;
using System.Text;
using SisoDb.Cryptography;
using SisoDb.Serialization;

namespace SisoDb
{
    public static class SisoDbEnvironment
    {
        public static readonly IFormatProvider FormatProvider;
        public static readonly IFormatProvider DateTimeFormatProvider;
        public static readonly string DateTimePattern;
        public static readonly Encoding Encoding;
        public static readonly IHashService HashService;
        public static readonly IMemberNameGenerator MemberNameGenerator;
        public static readonly IStringConverter StringConverter;
        public static readonly IJsonSerializer JsonSerializer;

        static SisoDbEnvironment()
        {
            FormatProvider = CultureInfo.InvariantCulture;
            DateTimeFormatProvider = new CultureInfo("sv-SE");
            DateTimePattern = "yyyy-MM-dd HH:mm:ss.FFFFFFFK";
            Encoding = Encoding.UTF8;
            HashService = new HashService();
            MemberNameGenerator = new HashMemberNameGenerator(HashService);
            StringConverter = new StringConverter(FormatProvider, DateTimeFormatProvider, DateTimePattern);
            JsonSerializer = new NewtonsoftJsonSerializer();
            //JsonSerializer = new ServiceStackJsonSerializer();
        }
    }
}