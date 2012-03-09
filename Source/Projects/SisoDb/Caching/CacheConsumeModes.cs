using System;

namespace SisoDb.Caching
{
	[Serializable]
	public enum CacheConsumeModes
	{
		UpdateCacheWithDbResult,
		DoNotUpdateCacheWithDbResult
	}
}