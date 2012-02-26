using System;
using System.Web;
using System.Web.Caching;
using SisoDb.Caching;

namespace SisoDb.AspWebCache
{
    /// <summary>
    /// Cache provider implemented over <see cref="http://msdn.microsoft.com/en-us/library/system.web.caching.cache.aspx"/>
    /// </summary>
    public class WebCacheProvider : CacheProviderBase
    {
	    private readonly Func<Cache> _httpCacheFn;
        private readonly Func<Type, WebCacheConfig> _webCacheConfigFn;

        public WebCacheProvider(Func<Cache> httpCacheFn = null, Func<Type, WebCacheConfig> webCacheConfigFn = null)
        {
            _httpCacheFn = httpCacheFn ?? (() => HttpContext.Current.Cache);
            _webCacheConfigFn = webCacheConfigFn ?? WebCacheConfig.CreateSliding;
        }

        protected override ICache OnCreate(Type structureType)
        {
            return new WebCache(_httpCacheFn.Invoke(), _webCacheConfigFn.Invoke(structureType));
        }
	}
}