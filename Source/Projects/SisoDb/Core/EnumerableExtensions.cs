using System.Collections;
using System.Collections.Generic;

namespace SisoDb.Core
{
	internal static class EnumerableExtensions
	{
		internal static IEnumerable<object> Yield(this IEnumerable items) //TODO: Move to NCore
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
	}
}