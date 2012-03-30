using System.Data;
using System.Data.Common;
using MvcMiniProfiler.Data;
using SisoDb.Dac.Profiling;

namespace SisoDb.MiniProfiler
{
    public class ProfiledConnectionWrapper : ProfiledDbConnection, IWrappedConnection
    {
        public ProfiledConnectionWrapper(DbConnection connection, IDbProfiler profiler)
            : base(connection, profiler)
        {
        }

        public IDbConnection GetInnerConnection()
        {
            return _conn;
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new ProfiledTransactionWrapper(_conn.BeginTransaction(isolationLevel), this);
        }
    }
}