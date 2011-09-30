using System;
using System.Collections.Generic;
using System.Linq;

namespace SisoDb.Core
{
    internal static class AssertArgsExtensions
    {
        internal static IEnumerable<T> AssertHasItems<T>(this IEnumerable<T> items, string name)
        {
            if (items == null || items.Count() < 1)
                throw new ArgumentNullException(name);

            return items;
        }

        internal static IList<T> AssertHasItems<T>(this IList<T> items, string name)
        {
            if (items == null || items.Count < 1)
                throw new ArgumentNullException(name);

            return items;
        }

        internal static T AssertNotNull<T>(this T item, string name) where T : class
        {
            if (item == null)
                throw new ArgumentNullException(name);

            return item;
        }

        internal static int AssertGt(this int value, int limit, string name)
        {
            if (value <= limit)
                throw new ArgumentOutOfRangeException(name, "value '{0}' is <= limit '{1}'.".Inject(value, limit));

            return value;
        }

        internal static int AssertGte(this int value, int limit, string name)
        {
            if (!(value >= limit))
                throw new ArgumentOutOfRangeException(name, "value '{0}' is not >= limit '{1}'.".Inject(value, limit));

            return value;
        }

        internal static int AssertInRange(this int value, int min, int max, string name)
        {
            if (value < min)
                throw new ArgumentOutOfRangeException(name, "value '{0}' is < min '{1}'.".Inject(value, min));


            if (value > max)
                throw new ArgumentOutOfRangeException(name, "value '{0}' is > max '{1}'.".Inject(value, max));

            return value;
        }

        internal static string AssertNotNullOrWhiteSpace(this string item, string name)
        {
            if (string.IsNullOrWhiteSpace(item))
                throw new ArgumentNullException(name);

            return item;
        }
    }
}