using System;
using System.Collections.Concurrent;
using SisoDb.Core;
using SisoDb.Resources;

namespace SisoDb.InProcCache
{
	public class InProcMemoryCacheProvider : ICacheProvider
	{
		private readonly ConcurrentDictionary<Type, ICache> _cache;

		public InProcMemoryCacheProvider()
		{
			_cache = new ConcurrentDictionary<Type, ICache>();
			AutoEnable = true;
		}

		public ICache this[Type structureType]
		{
			get
			{
				if (!AutoEnable && !_cache.ContainsKey(structureType))
					return null;

				return _cache.GetOrAdd(structureType, new InProcMemoryCache(structureType));
			}
		}

		public bool AutoEnable { get; set; }

		public void Clear()
		{
			var ex = _cache.Values.Try(e => e.Clear(), ExceptionMessages.InMemoryCacheProvider_Clear_failed);
			if (ex != null)
				throw ex;
		}

		public bool Handles(Type structureType)
		{
			return _cache.ContainsKey(structureType);
		}

		public void EnableFor(Type structureType)
		{
			_cache.GetOrAdd(structureType, new InProcMemoryCache(structureType));
		}

		public void DisableFor(Type structureType)
		{
			if (!_cache.ContainsKey(structureType))
				return;

			ICache temp;
			_cache.TryRemove(structureType, out temp);
		}
	}
}