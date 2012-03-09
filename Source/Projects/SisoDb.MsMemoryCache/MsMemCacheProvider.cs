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
        private readonly Func<Type, MemoryCache> _memoryCacheFn;
        private readonly Func<Type, MsMemCacheConfig> _memCacheConfigFn;

        public MsMemCacheProvider(Func<Type, MemoryCache> memoryCacheFn = null, Func<Type, MsMemCacheConfig> memCacheConfigFn = null)
        {
            _memoryCacheFn = memoryCacheFn ?? (t => new MemoryCache(t.Name));
            _memCacheConfigFn = memCacheConfigFn ?? MsMemCacheConfig.CreateSliding;
        }

        protected override ICache OnCreate(Type structureType)
        {
            return new MsMemCache(_memoryCacheFn.Invoke(structureType), _memCacheConfigFn.Invoke(structureType));
        }
    }
}