using System;

namespace SisoDb.Querying
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