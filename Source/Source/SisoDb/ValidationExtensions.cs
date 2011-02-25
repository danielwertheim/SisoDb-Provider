using System;
using System.Collections.Generic;
using System.Linq;

namespace SisoDb
{
    internal static class ValidationExtensions
    {
        internal static IEnumerable<T> AssertHasItems<T>(this IEnumerable<T> items, string name)
        {
            if(items == null || items.Count() < 1)
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

        internal static string AssertNotNullOrWhiteSpace(this string item, string name)
        {
            if (string.IsNullOrWhiteSpace(item))
                throw new ArgumentNullException(name);

            return item;
        }
    }
}