using System;
using System.Globalization;
using System.Text;
using SisoDb.Core;

namespace SisoDb
{
    public class SisoDbFormatting : ISisoDbFormatting
    {
        public IFormatProvider FormatProvider { get; private set; }
        public IFormatProvider DateTimeFormatProvider { get; private set; }
        public string DateTimePattern { get; private set; }
        public Encoding Encoding { get; private set; }
        public IStringConverter StringConverter { get; private set; }

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