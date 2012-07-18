using System;
using System.Globalization;
using System.Text;

namespace NCore
{
    public class DefaultFormatting : IFormatting
    {
        public IFormatProvider FormatProvider { get; private set; }
        public IFormatProvider DateTimeFormatProvider { get; private set; }
        public string DateTimePattern { get; private set; }
        public Encoding Encoding { get; private set; }

        public DefaultFormatting()
        {
            FormatProvider = CultureInfo.InvariantCulture;
            DateTimeFormatProvider = new CultureInfo("sv-SE").DateTimeFormat;
            DateTimePattern = "yyyy-MM-dd HH:mm:ss.FFFFFFFK";
            Encoding = Encoding.UTF8;
        }
    }
}