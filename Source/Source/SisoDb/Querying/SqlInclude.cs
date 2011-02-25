using System;

namespace SisoDb.Querying
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