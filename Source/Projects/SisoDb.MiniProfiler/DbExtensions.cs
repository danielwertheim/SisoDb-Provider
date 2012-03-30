using System.Data.Common;

namespace SisoDb.MiniProfiler
{
    public static class DbExtensions
    {
        public static void ActivateProfiler(this ISisoDatabase source)
        {

            var manager = source.ProviderFactory.ConnectionManager;
            manager.OnConnectionCreated = con => con is DbConnection
                ? new ProfiledConnectionWrapper((DbConnection)con, MvcMiniProfiler.MiniProfiler.Current)
                : con;
        }
    }
}
