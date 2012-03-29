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
            return IsEnabledFor(cacheProvider, structureSchema.Type.Type);
        }

        internal static bool IsEnabledFor(this ICacheProvider cacheProvider, Type structureType)
        {
            return (cacheProvider != null && cacheProvider.Handles(structureType));
        }

        internal static void NotifyOfPurge(this ICacheProvider cacheProvider, IStructureSchema structureSchema)
        {
            NotifyOfPurge(cacheProvider, structureSchema.Type.Type);
        }

        internal static void NotifyOfPurge(this ICacheProvider cacheProvider, Type structureType)
        {
            if (cacheProvider.IsEnabledFor(structureType))
                cacheProvider[structureType].Clear();
        }

        internal static void NotifyOfPurgeAll(this ICacheProvider cacheProvider)
        {
            if(cacheProvider != null)
                cacheProvider.Clear();
        }

        internal static void NotifyDeleting(this ICacheProvider cacheProvider, IStructureSchema structureSchema, IStructureId structureId)
        {
            if (!cacheProvider.IsEnabledFor(structureSchema))
                return;

            cacheProvider[structureSchema.Type.Type].Remove(structureId);
        }

        internal static void NotifyDeleting(this ICacheProvider cacheProvider, IStructureSchema structureSchema, IEnumerable<IStructureId> structureIds)
        {
            if (!cacheProvider.IsEnabledFor(structureSchema))
                return;

            cacheProvider[structureSchema.Type.Type].Remove(structureIds);
        }

        internal static bool Exists(this ICacheProvider cacheProvider, IStructureSchema structureSchema, IStructureId structureId, Func<IStructureId, bool> nonCacheQuery)
        {
            if (!cacheProvider.IsEnabledFor(structureSchema))
                return nonCacheQuery.Invoke(structureId);

            var cache = cacheProvider[structureSchema.Type.Type];
            return cache.Exists(structureId) || nonCacheQuery.Invoke(structureId);
        }

        internal static T Consume<T>(this ICacheProvider cacheProvider, IStructureSchema structureSchema, IStructureId structureId, Func<IStructureId, T> nonCacheQuery, CacheConsumeModes consumeMode) where T : class
        {
            if (!cacheProvider.IsEnabledFor(structureSchema))
                return nonCacheQuery.Invoke(structureId);

            var cache = cacheProvider[structureSchema.Type.Type];
            var structure = cache.GetById<T>(structureId);
            if (structure != null)
                return structure;

            structure = nonCacheQuery.Invoke(structureId);
            if (structure == null || consumeMode == CacheConsumeModes.DoNotUpdateCacheWithDbResult)
                return structure;

            return cache.Put(structureId, structure);
        }

        internal static IEnumerable<T> Consume<T>(this ICacheProvider cacheProvider, IStructureSchema structureSchema, IStructureId[] structureIds, Func<IStructureId[], IEnumerable<T>> nonCacheQuery, CacheConsumeModes consumeMode) where T : class
        {
            if (!cacheProvider.IsEnabledFor(structureSchema))
                return nonCacheQuery.Invoke(structureIds);

            var cache = cacheProvider[structureSchema.Type.Type];
            var cachedResult = cache.GetByIds<T>(structureIds);

            var cacheWasEmpty = cachedResult.Any() == false;
            if (cacheWasEmpty)
            {
                if (consumeMode == CacheConsumeModes.DoNotUpdateCacheWithDbResult)
                    return nonCacheQuery.Invoke(structureIds);

                return cache.Put(nonCacheQuery.Invoke(structureIds).ToDictionary(s => structureSchema.IdAccessor.GetValue(s)));
            }

            var allWasCached = cachedResult.Count == structureIds.Length;
            if (allWasCached)
                return cachedResult.Values;

            var deltaIds = structureIds.Where(sid => !cachedResult.ContainsKey(sid)).ToArray();
            if (!deltaIds.Any())
                return cachedResult.Values;

            if (consumeMode == CacheConsumeModes.DoNotUpdateCacheWithDbResult)
                return cachedResult.Values.MergeWith(nonCacheQuery.Invoke(deltaIds));

            return cachedResult.Values.MergeWith(cache.Put(nonCacheQuery.Invoke(deltaIds).ToDictionary(s => structureSchema.IdAccessor.GetValue(s))));
        }
    }
}