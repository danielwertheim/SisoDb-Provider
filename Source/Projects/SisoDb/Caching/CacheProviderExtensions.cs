using System;
using System.Collections.Generic;
using System.Linq;
using NCore.Collections;
using PineCone.Structures;
using PineCone.Structures.Schemas;

namespace SisoDb.Caching
{
	internal static class CacheProviderExtensions
	{
        internal static bool IsEnabledFor(this ICacheProvider cacheProvider, IStructureSchema structureSchema)
        {
            return (cacheProvider != null && cacheProvider.Handles(structureSchema.Type.Type));
        }

		internal static void NotifyDeleting(this ICacheProvider cacheProvider, IStructureSchema structureSchema, IStructureId structureId)
		{
			if(!cacheProvider.IsEnabledFor(structureSchema))
                return;

			cacheProvider[structureSchema.Type.Type].Remove(structureId);
		}

		internal static void NotifyDeleting(this ICacheProvider cacheProvider, IStructureSchema structureSchema, IEnumerable<IStructureId> structureIds)
		{
            if (!cacheProvider.IsEnabledFor(structureSchema))
                return;

			cacheProvider[structureSchema.Type.Type].Remove(structureIds);
		}

        internal static bool Exists<T>(this ICacheProvider cacheProvider, IStructureSchema structureSchema, IStructureId structureId, Func<IStructureId, bool> nonCacheQuery) where T : class
        {
            if (!cacheProvider.IsEnabledFor(structureSchema))
                return nonCacheQuery.Invoke(structureId);

            var cache = cacheProvider[structureSchema.Type.Type];

            return cache.Exists<T>(structureId);
        }

		internal static T Consume<T>(this ICacheProvider cacheProvider, IStructureSchema structureSchema, IStructureId structureId, Func<IStructureId, T> nonCacheQuery, CacheConsumeModes consumeMode) where T : class
		{
            if (!cacheProvider.IsEnabledFor(structureSchema))
				return nonCacheQuery.Invoke(structureId);

			var cache = cacheProvider[structureSchema.Type.Type];

			return cache.GetById<T>(structureId)
				?? (consumeMode == CacheConsumeModes.UpdateCacheWithDbResult 
				? cache.Put(structureId, nonCacheQuery.Invoke(structureId)) 
				: nonCacheQuery.Invoke(structureId));
		}

		internal static IEnumerable<T> Consume<T>(this ICacheProvider cacheProvider, IStructureSchema structureSchema, IStructureId[] structureIds, Func<IStructureId[], IEnumerable<T>> nonCacheQuery, CacheConsumeModes consumeMode) where T : class
		{
            if (!cacheProvider.IsEnabledFor(structureSchema))
				return nonCacheQuery.Invoke(structureIds);

			var cache = cacheProvider[structureSchema.Type.Type];
			var cachedResult = cache.GetByIds<T>(structureIds);

			if (!cachedResult.Any())
				return consumeMode == CacheConsumeModes.UpdateCacheWithDbResult
					? cache.Put(nonCacheQuery.Invoke(structureIds).ToDictionary(s => structureSchema.IdAccessor.GetValue(s)))
				    : nonCacheQuery.Invoke(structureIds);

			var allWasCached = cachedResult.Count == structureIds.Length;
			if (allWasCached)
				return cachedResult.Values;
			
			var deltaIds = structureIds.Where(sid => !cachedResult.ContainsKey(sid)).ToArray();
			if (consumeMode == CacheConsumeModes.UpdateCacheWithDbResult)
				return cachedResult.Values.MergeWith(cache.Put(nonCacheQuery.Invoke(deltaIds).ToDictionary(s => structureSchema.IdAccessor.GetValue(s))));

			return cachedResult.Values.MergeWith(nonCacheQuery.Invoke(deltaIds));
		}
	}
}