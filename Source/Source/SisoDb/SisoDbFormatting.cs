using System;
using System.Globalization;
using System.Text;

namespace SisoDb
{
    public class SisoDbFormatting
    {
        public readonly IFormatProvider FormatProvider;
        public readonly IFormatProvider DateTimeFormatProvider;
        public readonly string DateTimePattern;
        public readonly Encoding Encoding;
        public readonly IStringConverter StringConverter;

        public SisoDbFormatting()
        {
            FormatProvider = CultureInfo.InvariantCulture;
            DateTimeFormatProvider = new CultureInfo("sv-SE");
            DateTimePattern = "yyyy-MM-dd HH:mm:ss.FFFFFFFK";
            Encoding = Encoding.UTF8;
            StringConverter = new StringConverter(FormatProvider, DateTimeFormatProvider, DateTimePattern);
        }
    }
}