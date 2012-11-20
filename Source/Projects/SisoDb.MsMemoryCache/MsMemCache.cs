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
	    protected readonly MemoryCache InternalCache;
	    protected readonly MsMemCacheConfig CacheConfig;

        public MsMemCache(MemoryCache memoryCache, MsMemCacheConfig cacheConfig)
		{
            Ensure.That(memoryCache, "memoryCache").IsNotNull();
            Ensure.That(cacheConfig, "cacheConfig").IsNotNull();

            InternalCache = memoryCache;
	        CacheConfig = cacheConfig;
		}

		public virtual Type StructureType
		{
			get { return CacheConfig.StructureType; }
		}

		public virtual void Clear()
		{
            //TODO: http://msdn.microsoft.com/en-us/library/system.runtime.caching.memorycache.getenumerator.aspx
            //Would be better to InternalCache.Dispose() and create new
		    var cacheEnumerator = ((IEnumerable<KeyValuePair<string, object>>) InternalCache).GetEnumerator();
            while (cacheEnumerator.MoveNext())
            {
                if (cacheEnumerator.Current.Key.StartsWith(CacheConfig.CacheEntryKeyPrefix))
                    InternalCache.Remove(cacheEnumerator.Current.Key);
            }
		}

        public virtual bool Any()
	    {
	        return Count() > 0;
	    }

        public virtual long Count()
	    {
	        return InternalCache.GetCount();
	    }

	    public virtual bool HasQuery(string queryChecksum)
	    {
	        throw new NotImplementedException();
	    }

	    public virtual bool Exists(IStructureId id)
	    {
	        return Any() && InternalCache.Get(GenerateCacheKey(id)) != null;
	    }

	    public virtual IEnumerable<T> GetAll<T>() where T : class
	    {
	        return InternalCache.Select(kv => kv.Value as T);
	    }

	    public virtual IEnumerable<T> Query<T>(Expression<Func<T, bool>> predicate) where T : class
	    {
	        var e = predicate.Compile();

	        return InternalCache.Where(kv => e(kv.Value as T)).Select(kv => kv.Value as T);
	    }

	    public virtual T GetById<T>(IStructureId id) where T : class
	    {
	        return InternalCache.Get(GenerateCacheKey(id)) as T;
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
	        throw new NotImplementedException();
	    }

	    public virtual T Put<T>(IStructureId id, T structure) where T : class
		{
            InternalCache.Set(GenerateCacheKey(id), structure, CreateCacheItemPolicy());

			return structure;
		}

		public virtual IEnumerable<T> Put<T>(IEnumerable<KeyValuePair<IStructureId, T>> items) where T : class
		{
		    return items.Select(kv => Put(kv.Key, kv.Value));
		}

	    public virtual IEnumerable<T> Put<T>(string queryChecksum, IEnumerable<KeyValuePair<IStructureId, T>> items) where T : class
	    {
	        throw new NotImplementedException();
	    }

	    public virtual void Remove(IStructureId id)
		{
            InternalCache.Remove(GenerateCacheKey(id));
		}

		public virtual void Remove(IEnumerable<IStructureId> ids)
		{
			foreach (var structureId in ids)
				Remove(structureId);
		}

        protected virtual string GenerateCacheKey(IStructureId id)
        {
            return CacheConfig.CacheEntryKeyGenerator.Invoke(id);
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