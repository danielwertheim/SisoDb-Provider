using System;
using System.Text;

namespace SisoDb
{
    public interface ISisoDbFormatting
    {
        IFormatProvider FormatProvider { get; }
        IFormatProvider DateTimeFormatProvider { get; }
        string DateTimePattern { get; }
        Encoding Encoding { get; }
        IStringConverter StringConverter { get; }
    }
}