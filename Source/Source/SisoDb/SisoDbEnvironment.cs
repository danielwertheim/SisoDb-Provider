using System;
using System.Globalization;
using System.Text;
using SisoDb.Cryptography;

namespace SisoDb
{
    internal static class SisoDbEnvironment
    {
        internal static readonly IFormatProvider FormatProvider = CultureInfo.InvariantCulture;
        internal static readonly IFormatProvider DateTimeFormatProvider = new CultureInfo("sv-SE");
        internal static readonly string DateTimePattern = "yyyy-MM-dd HH:mm:ss.FFFFFFFK";
        internal static readonly Encoding Encoding = Encoding.UTF8;
        internal static readonly IHashService HashService = new HashService();
        internal static readonly IMemberNameGenerator MemberNameGenerator = new HashMemberNameGenerator(HashService);
        internal static IStringConverter StringConverter = new StringConverter();
    }
}