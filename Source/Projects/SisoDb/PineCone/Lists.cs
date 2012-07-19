using System.Collections.Generic;

namespace SisoDb.PineCone
{
    internal static class Lists
    {
        internal static IList<T> New<T>(T[] values)
        {
            var list = new List<T>(values.Length);

            list.AddRange(values);

            return list;
        }

        internal static IList<T> Empty<T>()
        {
            return new List<T>();
        }
    }
}