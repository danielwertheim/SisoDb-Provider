using System.Collections.Generic;
using System.Linq;

namespace SisoDb.Core
{
    public static class StringExtensions
    {
         public static string ToJoinedString(this IEnumerable<string> strings, string separator, string itemFormat)
         {
             return string.Join(separator, strings.Select(s => string.Format(itemFormat, s)));
         }
    }
}