using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SisoDb.NCore.Collections;
using SisoDb.Querying.Sql;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Caching
{
    /// <summary>
    /// Helpers that helps you interact with the Db-Cache, with regards to
    /// keeping it up-to-date.
    /// </summary>
    public static class CacheProviderExtensions
    {
        public static bool IsEnabledFor(this ICacheProvider cacheProvider, IStructureSchema structureSchema)
        {
            return IsEnabledFor(cacheProvider, structureSchema.Type.Type);
        }

        public static bool IsEnabledFor(this ICacheProvider cacheProvider, Type structureType)
        {
            return (cacheProvider != null && cacheProvider.Handles(structureType));
        }

        public static void ClearAll(this ICacheProvider cacheProvider)
        {
            if (cacheProvider != null)
                cacheProvider.Clear();
        }

        public static void ClearByType(this ICacheProvider cacheProvider, IStructureSchema structureSchema)
        {
            ClearByType(cacheProvider, structureSchema.Type.Type);
        }

        public static void ClearByType(this ICacheProvider cacheProvider, Type structureType)
        {
            if (cacheProvider.IsEnabledFor(structureType))
                cacheProvider[structureType].Clear();
        }

        public static void CleanQueriesFor(this ICacheProvider cacheProvider, IStructureSchema structureSchema)
        {
            if (!cacheProvider.IsEnabledFor(structureSchema))
                return;

            cacheProvider[structureSchema.Type.Type].ClearQueries();
        }

        public static void Remove(this ICacheProvider cacheProvider, IStructureSchema structureSchema, IStructureId structureId)
        {
            if (!cacheProvider.IsEnabledFor(structureSchema))
                return;

            cacheProvider[structureSchema.Type.Type].Remove(structureId);
        }

        public static void Remove(this ICacheProvider cacheProvider, IStructureSchema structureSchema, ISet<IStructureId> structureIds)
        {
            if (!cacheProvider.IsEnabledFor(structureSchema))
                return;

            cacheProvider[structureSchema.Type.Type].Remove(structureIds);
        }

        public static bool Any(this ICacheProvider cacheProvider, IStructureSchema structureSchema, Func<IStructureSchema, bool> nonCacheQuery)
        {
            if (!cacheProvider.IsEnabledFor(structureSchema))
                return nonCacheQuery.Invoke(structureSchema);

            return cacheProvider[structureSchema.Type.Type].Count() > 0 || nonCacheQuery.Invoke(structureSchema);
        }

        public static bool Exists(this ICacheProvider cacheProvider, IStructureSchema structureSchema, IStructureId structureId, Func<IStructureId, bool> nonCacheQuery)
        {
            if (!cacheProvider.IsEnabledFor(structureSchema))
                return nonCacheQuery.Invoke(structureId);

            return cacheProvider[structureSchema.Type.Type].Exists(structureId) || nonCacheQuery.Invoke(structureId);
        }

        public static T Consume<T>(this ICacheProvider cacheProvider, IStructureSchema structureSchema, Expression<Func<T, bool>> predicate, Func<Expression<Func<T, bool>>, T> nonCacheQuery, CacheConsumeModes consumeMode) where T : class
        {
            if (!cacheProvider.IsEnabledFor(structureSchema))
                return nonCacheQuery.Invoke(predicate);

            var cache = cacheProvider[structureSchema.Type.Type];
            var structure = cache.Query(predicate).SingleOrDefault();
            if (structure != null)
                return structure;

            structure = nonCacheQuery.Invoke(predicate);
            if (structure == null || consumeMode == CacheConsumeModes.DoNotUpdateCacheWithDbResult)
                return structure;

            return cacheProvider[structureSchema.Type.Type].Put(structureSchema.IdAccessor.GetValue(structure), structure);
        }

        public static T Consume<T>(this ICacheProvider cacheProvider, IStructureSchema structureSchema, IStructureId structureId, Func<IStructureId, T> nonCacheQuery, CacheConsumeModes consumeMode) where T : class
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

        public static IEnumerable<T> Consume<T>(this ICacheProvider cacheProvider, IStructureSchema structureSchema, IStructureId[] structureIds, Func<IStructureId[], IEnumerable<T>> nonCacheQuery, CacheConsumeModes consumeMode) where T : class
        {
            if (!cacheProvider.IsEnabledFor(structureSchema))
                return nonCacheQuery.Invoke(structureIds);

            var cache = cacheProvider[structureSchema.Type.Type];
            var cachedResult = cache.GetByIds<T>(structureIds).ToDictionary(kv => kv.Key, kv => kv.Value); //Ok to turn it to in-mem rep and not yield. GetByIds, should not be enormous resultset.

            if (!cachedResult.Any())
            {
                if (consumeMode == CacheConsumeModes.DoNotUpdateCacheWithDbResult)
                    return nonCacheQuery.Invoke(structureIds);

                return cache.Put(nonCacheQuery.Invoke(structureIds).Select(s => new KeyValuePair<IStructureId, T>(structureSchema.IdAccessor.GetValue(s), s)));
            }

            var allWasCached = cachedResult.Count == structureIds.Length;
            if (allWasCached)
                return cachedResult.Values;

            var deltaIds = structureIds.Where(sid => !cachedResult.ContainsKey(sid)).ToArray();
            if (!deltaIds.Any())
                return cachedResult.Values;

            if (consumeMode == CacheConsumeModes.DoNotUpdateCacheWithDbResult)
                return cachedResult.Values.MergeWith(nonCacheQuery.Invoke(deltaIds));

            return cachedResult.Values.MergeWith(cache.Put(nonCacheQuery.Invoke(deltaIds).Select(s => new KeyValuePair<IStructureId, T>(structureSchema.IdAccessor.GetValue(s), s))));
        }
        
        public static IEnumerable<T> Consume<T>(this ICacheProvider cacheProvider, IStructureSchema structureSchema, IDbQuery query, Func<IDbQuery, IEnumerable<T>> nonCacheQuery, CacheConsumeModes consumeMode) where T : class
        {
            if (!cacheProvider.IsEnabledFor(structureSchema))
                return nonCacheQuery.Invoke(query);

            var queryChecksum = DbQueryChecksumGenerator.Instance.Generate(query);
            var cache = cacheProvider[structureSchema.Type.Type];
            if (cache.HasQuery(queryChecksum))
                return cache.GetByQuery<T>(queryChecksum);

            if (!query.IsCacheable || consumeMode == CacheConsumeModes.DoNotUpdateCacheWithDbResult)
                return nonCacheQuery.Invoke(query);

            return cache.Put(
                queryChecksum, 
                nonCacheQuery.Invoke(query).Select(s => new KeyValuePair<IStructureId, T>(structureSchema.IdAccessor.GetValue(s), s)));
        }
    }
}