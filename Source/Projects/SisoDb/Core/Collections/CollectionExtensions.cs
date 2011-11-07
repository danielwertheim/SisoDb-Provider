using System.Collections.Generic;
using System.Linq;

namespace SisoDb.Core.Collections
{
    public static class CollectionExtensions
    {
        public static IEnumerable<T> MergeWith<T>(this IEnumerable<T> set1, IEnumerable<T> set2)
        {
            foreach (var element in set1)
                yield return element;

            foreach (var element in set2)
                yield return element;
        }

        public static IEnumerable<T> MergeDistinctWith<T>(this IEnumerable<T> set1, IEnumerable<T> set2)
        {
            var yielded = new HashSet<T>();

            foreach (var element in set1)
            {
                yielded.Add(element);
                yield return element;
            }

            foreach (var element in set2.Where(e => !yielded.Contains(e)))
                yield return element;

            yielded.Clear();
        }
    }
}