using System;
using System.Collections.Generic;

namespace SisoDb.TestUtils
{
    public static class ListExtensions
    {
        public static IList<T> New<T>(int numOfItems, Action<int, T> intercept = null) where T : new()
        {
            var items = new List<T>();

            for (var c = 0; c < numOfItems; c++)
            {
                var item = new T();
                if (intercept != null)
                    intercept(c, item);

                items.Add(item);
            }

            return items;
        }
    }
}