using System;
using SisoDb.Core;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlInclude : ISqlInclude
    {
        public string Sql { get; private set; }

        public SqlInclude(string sql)
        {
            Sql = sql.AssertNotNullOrWhiteSpace("sql");
        }
    }
}