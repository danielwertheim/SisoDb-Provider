using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NCore.Collections
{
    public static class EnumerableExtensions
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

		public static T PeekLeft<T>(this T[] source, int currentIndex) where T : class
		{
			return currentIndex > 0
				? source[currentIndex - 1]
				: null;
		}

		public static T PeekRight<T>(this T[] source, int currentIndex) where T : class
		{
			return currentIndex < source.Length - 1
				? source[currentIndex + 1]
				: null;
		}

		public static IEnumerable<T[]> Batch<T>(this IEnumerable<T> source, int maxBatchSize)
		{
			var batch = new List<T>(maxBatchSize);

			foreach (var item in source)
			{
				batch.Add(item);
				if (batch.Count < maxBatchSize)
					continue;

				yield return batch.ToArray();
				batch.Clear();
			}

			if (batch.Count > 0)
			{
				yield return batch.ToArray();
				batch.Clear();
			}
		}

		public static IEnumerable<TResult[]> Batch<T, TResult>(this IEnumerable<T> source, int maxBatchSize, Func<T, TResult> transform)
		{
			var batch = new List<TResult>(maxBatchSize);

			foreach (var item in source)
			{
				batch.Add(transform(item));
				if (batch.Count < maxBatchSize)
					continue;

				yield return batch.ToArray();
				batch.Clear();
			}

			if (batch.Count > 0)
			{
				yield return batch.ToArray();
				batch.Clear();
			}
		}

		public static IEnumerable<TResult[]> Batch<T, TResult>(this IEnumerable<T> source, int maxBatchSize, Func<T, int, TResult> transform)
		{
			var batch = new List<TResult>(maxBatchSize);

			foreach (var item in source)
			{
				batch.Add(transform(item, batch.Count));
				if (batch.Count < maxBatchSize)
					continue;

				yield return batch.ToArray();
				batch.Clear();
			}

			if (batch.Count > 0)
			{
				yield return batch.ToArray();
				batch.Clear();
			}
		}

        public static IEnumerable<object> Yield(this IEnumerable items)
        {
            if (items == null)
                yield break;

            foreach (var i in items)
            {
                if (i is IEnumerable && !(i is string))
                    foreach (var o in Yield(i as IEnumerable))
                        yield return o;
                else
                    yield return i;
            }
        }

        public static Exception[] TryForAll<T>(this IEnumerable<T> source, Action<T> action)
        {
            var exceptions = new List<Exception>();

            foreach (var element in source)
            {
                try
                {
                    action.Invoke(element);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            return exceptions.Any()
                ? exceptions.ToArray()
                : null;
        }
    }
}