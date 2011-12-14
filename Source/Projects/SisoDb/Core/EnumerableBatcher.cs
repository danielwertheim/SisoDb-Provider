using System;
using System.Collections.Generic;

namespace SisoDb.Core
{
	public static class EnumerableBatcher //TODO: Move to NCore
	{
		public static IEnumerable<T[]> Batch<T>(this IEnumerable<T> source, int maxBatchSize)
		{
			var batch = new List<T>(maxBatchSize);

			foreach (var item in source)
			{
				batch.Add(item);
				if(batch.Count < maxBatchSize)
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
	}
}