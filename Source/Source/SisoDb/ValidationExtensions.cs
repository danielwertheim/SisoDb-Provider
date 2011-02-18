using System;
using System.Collections.Generic;
using System.Linq;

namespace SisoDb
{
    internal static class ValidationExtensions
    {
        public static IEnumerable<T> AssertHasItems<T>(this IEnumerable<T> items, string name)
        {
            if(items == null || items.Count() < 1)
                throw new ArgumentNullException(name);

            return items;
        }

        public static IList<T> AssertHasItems<T>(this IList<T> items, string name)
        {
            if (items == null || items.Count < 1)
                throw new ArgumentNullException(name);

            return items;
        }

        public static T AssertNotNull<T>(this T item, string name) where T : class
        {
            if (item == null)
                throw new ArgumentNullException(name);

            return item;
        }

        public static string AssertNotNullOrWhiteSpace(this string item, string name)
        {
            if (string.IsNullOrWhiteSpace(item))
                throw new ArgumentNullException(name);

            return item;
        }
    }
}