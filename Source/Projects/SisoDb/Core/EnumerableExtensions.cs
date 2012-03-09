using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SisoDb.Core
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<object> Yield(this IEnumerable items) //TODO: Move to NCore
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
		public static SisoDbException Try<T>(this IEnumerable<T> source, Action<T> action, string errorMessage) where T : class
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
			if (!exceptions.Any())
				return null;
			return new SisoDbException(errorMessage, exceptions);
		}
	}
}