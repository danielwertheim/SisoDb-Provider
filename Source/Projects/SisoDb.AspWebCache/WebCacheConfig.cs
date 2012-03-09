using System;
using System.Web.Caching;
using EnsureThat;
using NCore;
using PineCone.Structures;

namespace SisoDb.AspWebCache
{
    public class WebCacheConfig
    {
        public const int DefaultSlidingExpirationSeconds = 3600;

        public Type StructureType { get; private set; }
        public DateTime AbsoluteExpiration { get; private set; }
        public TimeSpan SlidingExpiration { get; private set; }
        public string CacheEntryKeyPrefix { get; private set; }
        public Func<IStructureId, string> CacheEntryKeyGenerator { get; private set; }

        private WebCacheConfig(Type structureType, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            Ensure.That(structureType, "structureType").IsNotNull();

            StructureType = structureType;
            CacheEntryKeyPrefix = string.Concat(structureType.Name, ":");
            CacheEntryKeyGenerator = id => string.Concat(CacheEntryKeyPrefix, id.Value.ToString());

            AbsoluteExpiration = absoluteExpiration;
            SlidingExpiration = slidingExpiration;
        }

        public static WebCacheConfig CreateAbsolute(Type structureType, TimeSpan expiresIn)
        {
            return new WebCacheConfig(structureType, SysDateTime.Now.Add(expiresIn), Cache.NoSlidingExpiration);
        }

        public static WebCacheConfig CreateSliding(Type structureType)
        {
            return new WebCacheConfig(structureType, Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(DefaultSlidingExpirationSeconds));
        }

        public static WebCacheConfig CreateSliding(Type structureType, TimeSpan expiresIn)
        {
            return new WebCacheConfig(structureType, Cache.NoAbsoluteExpiration, expiresIn);
        }
    }
}