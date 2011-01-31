using System;
using System.Collections.Generic;

namespace SisoDb.Querying
{
    public static class EnumerableQueryExtensions
    {
        public static bool QxAny<T>(this IEnumerable<T> m, Func<T, bool> e)
        {
            throw new NotImplementedException();
        }
    }
}