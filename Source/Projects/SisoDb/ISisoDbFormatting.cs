using System;
using System.Text;
using SisoDb.Core;

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