using System;
using System.Runtime.Caching;
using SisoDb.Caching;

namespace SisoDb.MsMemoryCache
{
    /// <summary>
    /// Cache provider implemented over <![CDATA[http://msdn.microsoft.com/en-us/library/dd780634.aspx]]>
    /// </summary>
    public class MsMemCacheProvider : CacheProviderBase
    {
        protected readonly Func<Type, MemoryCache> MemoryCacheFn;
        protected readonly Func<Type, MsMemCacheConfig> MemCacheConfigFn;

        public MsMemCacheProvider(Func<Type, MemoryCache> memoryCacheFn = null, Func<Type, MsMemCacheConfig> memCacheConfigFn = null)
        {
            MemoryCacheFn = memoryCacheFn ?? (t => new MemoryCache(t.Name));
            MemCacheConfigFn = memCacheConfigFn ?? MsMemCacheConfig.CreateSliding;
        }

        protected override ICache OnCreate(Type structureType)
        {
            return new MsMemCache(MemoryCacheFn.Invoke(structureType), MemCacheConfigFn.Invoke(structureType));
        }
    }
}