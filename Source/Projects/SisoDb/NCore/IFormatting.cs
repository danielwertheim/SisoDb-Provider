using System;
using System.Text;

namespace SisoDb.NCore
{
    public interface IFormatting 
    {
        IFormatProvider FormatProvider { get; }
        IFormatProvider DateTimeFormatProvider { get; }
        string DateTimePattern { get; }
        Encoding Encoding { get; }
    }
}