using System;
using SisoDb.NCore;

namespace SisoDb
{
    public static class Sys
    {
        public static IFormatting Formatting = new DefaultFormatting();
        public static IStringConverter StringConverter = new StringConverter(Formatting);
        public static StringComparer StringComparer = StringComparer.OrdinalIgnoreCase;
        public static StringComparison StringComparision = StringComparison.OrdinalIgnoreCase;
    }
}