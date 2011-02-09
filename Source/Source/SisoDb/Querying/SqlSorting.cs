using System;

namespace SisoDb.Querying
{
    [Serializable]
    internal class SqlSorting : ISqlSorting
    {
        public string Sql { get; private set; }

        internal SqlSorting(string sql)
        {
            Sql = sql.AssertNotNullOrWhiteSpace("sql");
        }
    }
}