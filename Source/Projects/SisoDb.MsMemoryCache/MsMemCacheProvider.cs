using System;
using SisoDb.Caching;

namespace SisoDb.MsMemoryCache
{
    /// <summary>
    /// Cache provider implemented over <![CDATA[http://msdn.microsoft.com/en-us/library/dd780634.aspx]]>
    /// </summary>
    public class MsMemCacheProvider : CacheProviderBase
    {
        protected readonly Func<Type, MsMemCacheConfig> MemCacheConfigFn;

        public MsMemCacheProvider(Func<Type, MsMemCacheConfig> memCacheConfigFn = null)
        {
            MemCacheConfigFn = memCacheConfigFn ?? MsMemCacheConfig.CreateSliding;
        }

        protected override ICache OnCreate(Type structureType)
        {
            return new MsMemCache(MemCacheConfigFn.Invoke(structureType));
        }
    }
}