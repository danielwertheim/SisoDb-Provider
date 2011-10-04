using System;
using EnsureThat;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlSorting : ISqlSorting
    {
        public string Sql { get; private set; }

        public SqlSorting(string sql)
        {
            Ensure.That(() => sql).IsNotNullOrWhiteSpace();
            Sql = sql;
        }
    }
}