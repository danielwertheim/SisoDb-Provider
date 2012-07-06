using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SisoDb.Core
{
	public static class EnumerableExtensions
	{
		public static SisoDbException TryAll<T>(this IEnumerable<T> source, Action<T> action, string errorMessage) where T : class
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