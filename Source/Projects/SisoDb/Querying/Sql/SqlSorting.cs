using System;
using SisoDb.Core;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlSorting : ISqlSorting
    {
        public string Sql { get; private set; }

        public SqlSorting(string sql)
        {
            Sql = sql.AssertNotNullOrWhiteSpace("sql");
        }
    }
}