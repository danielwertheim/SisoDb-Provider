using System.Data;
using System.Data.SqlClient;

namespace SisoDb.Core
{
    public static class ConnectionExtensions
    {

        public static SqlConnection ToSqlConnection(this IDbConnection source)
        {
            var wrapped = source as IWrappedConnection;
            return (SqlConnection)(wrapped == null ? source : wrapped.GetInnerConnection());
        }

        public static SqlTransaction ToSqlTransaction(this IDbTransaction source)
        {
            var wrapped = source as IWrappedTransaction;
            return (SqlTransaction)(wrapped == null ? source : wrapped.GetInnerTransaction());
        }
    }
}
