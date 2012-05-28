using System;
using System.Linq;

namespace SisoDb.SqlCe4
{
    internal static class SqlStatementStringExtensions
    {
        internal static bool IsMultiStatement(this string sqlStatement)
        {
            var first = sqlStatement.IndexOf(';');
            var last = sqlStatement.LastIndexOf(';');
            return first > -1 && last > first;
        }
        internal static string[] ToSqlStatements(this string sqlStatement)
        {
            return sqlStatement.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
        }
    }
}