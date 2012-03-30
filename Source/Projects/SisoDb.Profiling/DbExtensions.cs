using System.Data.Common;
using MvcMiniProfiler;

namespace SisoDb.Profiling
{
    public static class DbExtensions
    {
        public static void ActivateProfiler(this ISisoDatabase source)
        {
            var manager = source.ProviderFactory.ConnectionManager;
            manager.OnConnectionCreated = con => con is DbConnection
                ? new ProfiledConnectionWrapper((DbConnection)con, MiniProfiler.Current)
                : con;
        }
    }
}
