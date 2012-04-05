using System.Data.Common;

namespace SisoDb.MiniProfiler
{
    public static class DbExtensions
    {
        public static void ActivateProfiler(this ISisoDatabase db)
        {
            db.ProviderFactory.ConnectionManager.OnConnectionCreated = 
                con => con is DbConnection
                    ? new ProfiledConnectionWrapper((DbConnection)con, StackExchange.Profiling.MiniProfiler.Current)
                    : con;
        }

        public static void DeactivateProfiler(this ISisoDatabase db)
        {
            db.ProviderFactory.ConnectionManager.ResetOnConnectionCreated();
        }
    }
}
