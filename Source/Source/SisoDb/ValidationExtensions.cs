using System;

namespace SisoDb
{
    internal static class ValidationExtensions
    {
        internal static T AssertNotNull<T>(this T item, string name) where T : class
        {
            if (item == null)
                throw new ArgumentNullException(name);

            return item;
        }
    }
}