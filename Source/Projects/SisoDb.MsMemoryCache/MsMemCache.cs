using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;
using SisoDb.EnsureThat;
using SisoDb.Structures;

namespace SisoDb.MsMemoryCache
{
	[Serializable]
	public class MsMemCache : ICache
	{
	    protected MemoryCache InternalStructureCache;
        protected MemoryCache InternalQueryCache;
	    protected readonly MsMemCacheConfig CacheConfig;

        public MsMemCache(MsMemCacheConfig cacheConfig)
		{
            Ensure.That(cacheConfig, "cacheConfig").IsNotNull();

            CacheConfig = cacheConfig;
            InternalStructureCache = CreateStructureCache();
            InternalQueryCache = CreateQueryCache();
		}

		public virtual Type StructureType
		{
			get { return CacheConfig.StructureType; }
		}

        protected MemoryCache CreateStructureCache()
        {
            return new MemoryCache(string.Concat("SisoDb", CacheConfig.StructureType.Name));
        }

        protected MemoryCache CreateQueryCache()
        {
            return new MemoryCache(string.Concat("SisoDb", CacheConfig.StructureType.Name, ":", "Queries"));
        }

		public virtual void Clear()
		{
            InternalStructureCache.Dispose();
		    InternalStructureCache = CreateStructureCache();

            InternalQueryCache.Dispose();
		    InternalQueryCache = CreateQueryCache();
		}

        public virtual bool Any()
	    {
	        return Count() > 0;
	    }

        public virtual long Count()
	    {
	        return InternalStructureCache.GetCount();
	    }

	    public virtual bool HasQuery(string queryChecksum)
	    {
	        return InternalQueryCache.Contains(queryChecksum);
	    }

	    public virtual bool Exists(IStructureId id)
	    {
	        return Any() && InternalStructureCache.Get(GenerateCacheKey(id)) != null;
	    }

	    public virtual IEnumerable<T> GetAll<T>() where T : class
	    {
	        return InternalStructureCache.Select(kv => kv.Value as T);
	    }

	    public virtual IEnumerable<T> Query<T>(Expression<Func<T, bool>> predicate) where T : class
	    {
	        var e = predicate.Compile();

	        return InternalStructureCache.Where(kv => e(kv.Value as T)).Select(kv => kv.Value as T);
	    }

	    public virtual T GetById<T>(IStructureId id) where T : class
	    {
	        return InternalStructureCache.Get(GenerateCacheKey(id)) as T;
		}

		public virtual IDictionary<IStructureId, T> GetByIds<T>(IStructureId[] ids) where T : class
		{
			var result = new Dictionary<IStructureId, T>(ids.Length);

			foreach (var id in ids)
			{
				var structure = GetById<T>(id);
				if (structure != null)
					result.Add(id, structure);
			}

			return result;
		}

	    public virtual IEnumerable<T> GetByQuery<T>(string queryChecksum) where T : class
	    {
	        var ids = InternalQueryCache.Get(queryChecksum) as List<IStructureId>;
	        if (ids == null) yield break;

	        foreach (var id in ids)
	            yield return GetById<T>(id);
	    }

	    public virtual T Put<T>(IStructureId id, T structure) where T : class
		{
            InternalStructureCache.Set(GenerateCacheKey(id), structure, CreateCacheItemPolicy());

			return structure;
		}

		public virtual IEnumerable<T> Put<T>(IEnumerable<KeyValuePair<IStructureId, T>> items) where T : class
		{
		    return items.Select(kv => Put(kv.Key, kv.Value));
		}

	    public virtual IEnumerable<T> Put<T>(string queryChecksum, IEnumerable<KeyValuePair<IStructureId, T>> items) where T : class
	    {
	        var ids = new List<IStructureId>();

	        foreach (var kv in items)
	        {
	            Put(kv.Key, kv.Value);
                ids.Add(kv.Key);
	            
                yield return kv.Value;
	        }

            InternalQueryCache.Set(queryChecksum, ids, CreateCacheItemPolicy());
	    }

	    public virtual void Remove(IStructureId id)
		{
            InternalStructureCache.Remove(GenerateCacheKey(id));
		}

		public virtual void Remove(IEnumerable<IStructureId> ids)
		{
			foreach (var structureId in ids)
				Remove(structureId);
		}

        protected virtual string GenerateCacheKey(IStructureId id)
        {
            return id.Value.ToString();
        }

        protected virtual CacheItemPolicy CreateCacheItemPolicy()
        {
            return new CacheItemPolicy
            {
                AbsoluteExpiration = CacheConfig.AbsoluteExpiration,
                SlidingExpiration = CacheConfig.SlidingExpiration
            };
        }
	}
}