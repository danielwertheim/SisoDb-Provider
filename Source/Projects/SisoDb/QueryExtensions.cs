using System;
using System.Collections.Generic;

namespace SisoDb
{
    public static class SortingExtensions
    {
        public static T Asc<T>(this T t)
        {
            return t;
        }

        public static T Desc<T>(this T t)
        {
            return t;
        }
    }

    public static class EnumerableQueryExtensions
    {
        public static bool QxAny<T>(this IEnumerable<T> m, Func<T, bool> e)
        {
            throw new NotImplementedException();
        }
    }

    public static class StringQueryExtensions
    {
        public static bool QxLike(this string m, string value)
        {
            throw new NotImplementedException();
        }

        public static bool QxStartsWith(this string m, string value)
        {
            throw new NotImplementedException();
        }
        
        public static bool QxEndsWith(this string m, string value)
        {
            throw new NotImplementedException();
        }

        public static bool QxContains(this string m, string value)
        {
            throw new NotImplementedException();
        }

        public static string QxToLower(this string m)
        {
            throw new NotImplementedException();
        }

        public static string QxToUpper(this string m)
        {
            throw new NotImplementedException();
        }
    }
}