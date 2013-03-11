using System.Data;
using System.Data.Common;
using SisoDb.Dac.Profiling;
using StackExchange.Profiling.Data;

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
            return _connection;
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new ProfiledTransactionWrapper(_connection.BeginTransaction(isolationLevel), this);
        }
    }
}