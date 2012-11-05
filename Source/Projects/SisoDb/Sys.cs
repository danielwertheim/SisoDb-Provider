using System;
using SisoDb.NCore;

namespace SisoDb
{
    public static class Sys
    {
        public static readonly IFormatting Formatting = new DefaultFormatting();

        public static readonly IStringConverter StringConverter = new StringConverter(Formatting);

        public static readonly StringComparer StringComparer = StringComparer.OrdinalIgnoreCase;

    	public static readonly StringComparison StringComparision = StringComparison.OrdinalIgnoreCase;
    }
}