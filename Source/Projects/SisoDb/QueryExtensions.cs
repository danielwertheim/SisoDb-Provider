using System;
using System.Collections.Generic;

namespace SisoDb
{
    public static class EnumerableQueryExtensions
    {
        public static bool QxAny(this IEnumerable<string> m)
        {
            throw new NotImplementedException();
        }

        public static bool QxAny<T>(this IEnumerable<T> m) where T : struct 
        {
            throw new NotImplementedException();
        }

        public static bool QxAny<T>(this IEnumerable<T?> m) where T : struct
        {
            throw new NotImplementedException();
        }

        public static bool QxAny<T>(this IEnumerable<T> m, Func<T, bool> e)
        {
            throw new NotImplementedException();
        }

        public static bool QxIn(this IEnumerable<string> m, params string[] s)
        {
            throw new NotImplementedException();
        }

        public static bool QxIn<T>(this IEnumerable<T> m, params T[] s) where T : struct 
        {
            throw new NotImplementedException();
        }

        public static bool QxIn<T>(this IEnumerable<T?> m, params T?[] s) where T : struct
        {
            throw new NotImplementedException();
        }

        public static bool QxNotIn(this IEnumerable<string> m, params string[] s)
        {
            throw new NotImplementedException();
        }

        public static bool QxNotIn<T>(this IEnumerable<T> m, params T[] s) where T : struct
        {
            throw new NotImplementedException();
        }

        public static bool QxNotIn<T>(this IEnumerable<T?> m, params T?[] s) where T : struct
        {
            throw new NotImplementedException();
        }
    }

    public static class SingleValueTypeQueryExtensions
    {
        public static bool QxIn(this string m, params string[] s)
        {
            throw new NotImplementedException();
        }

        public static bool QxIn<T>(this T m, params T[] s) where T : struct 
        {
            throw new NotImplementedException();
        }

        public static bool QxIn<T>(this T? m, params T?[] s) where T : struct
        {
            throw new NotImplementedException();
        }

        public static bool QxNotIn(this string m, params string[] s)
        {
            throw new NotImplementedException();
        }

        public static bool QxNotIn<T>(this T m, params T[] s) where T : struct
        {
            throw new NotImplementedException();
        }

        public static bool QxNotIn<T>(this T? m, params T?[] s) where T : struct
        {
            throw new NotImplementedException();
        }
    }

    public static class StringQueryExtensions
    {
        public static bool QxIsExactly(this string m, string value)
        {
            throw new NotImplementedException();
        }

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
            return m.ToLower();
        }

        public static string QxToUpper(this string m)
        {
            return m.ToUpper();
        }
    }
}