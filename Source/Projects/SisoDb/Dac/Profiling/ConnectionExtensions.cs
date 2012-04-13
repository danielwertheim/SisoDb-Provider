using System.Data;
using System.Data.SqlClient;

namespace SisoDb.Dac.Profiling
{
    public static class ConnectionExtensions
    {
        public static SqlConnection ToSqlConnection(this IDbConnection connection)
        {
            if (!(connection is IWrappedConnection))
                return (SqlConnection)connection;

            return (SqlConnection)((IWrappedConnection)connection).GetInnerConnection();
        }

        public static SqlTransaction ToSqlTransaction(this IDbTransaction transaction)
        {
            if (transaction == null)
                return null;

            if (!(transaction is IWrappedTransaction))
                return (SqlTransaction)transaction;

            return (SqlTransaction)((IWrappedTransaction)transaction).GetInnerTransaction();
        }
    }
}
