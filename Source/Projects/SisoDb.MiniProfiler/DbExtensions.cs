using System.Data.Common;

namespace SisoDb.MiniProfiler
{
    public static class DbExtensions
    {
        public static void ActivateProfiler(this ISisoDatabase db)
        {
            var manager = db.ProviderFactory.ConnectionManager;
            manager.OnConnectionCreated = con => con is DbConnection
                ? new ProfiledConnectionWrapper((DbConnection)con, MvcMiniProfiler.MiniProfiler.Current)
                : con;
        }

        public static void DeactivateProfiler(this ISisoDatabase db)
        {
            db.ProviderFactory.ConnectionManager.ResetOnConnectionCreated();
        }
    }
}
