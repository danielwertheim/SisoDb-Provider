using System.Data.Common;

namespace SisoDb.MiniProfiler
{
    public static class DbExtensions
    {
        public static void ActivateProfiler(this ISisoDatabase source)
        {
            var db = source as ISisoDbDatabase;
            if (db == null)
            {
                return;
            }
            var manager = db.ProviderFactory.ConnectionManager;
            manager.OnConnectionCreated = con => con is DbConnection
                ? new ProfiledConnectionWrapper((DbConnection)con, MvcMiniProfiler.MiniProfiler.Current)
                : con;
        }
    }
}
