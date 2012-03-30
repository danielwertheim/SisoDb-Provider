using System.Data;
using System.Data.SqlServerCe;
using SisoDb.Dac.Profiling;

namespace SisoDb.SqlCe4.Dac.Profiling
{
    internal static class ConnectionExtensions
    {
        internal static SqlCeConnection ToSqlCeConnection(this IDbConnection connection)
        {
            if (!(connection is IWrappedConnection))
                return (SqlCeConnection)connection;

            return (SqlCeConnection)((IWrappedConnection)connection).GetInnerConnection();
        }

        internal static SqlCeTransaction ToSqlCeTransaction(this IDbTransaction transaction)
        {
            if (!(transaction is IWrappedTransaction))
                return (SqlCeTransaction)transaction;

            return (SqlCeTransaction)((IWrappedTransaction)transaction).GetInnerTransaction();
        }
    }
}
