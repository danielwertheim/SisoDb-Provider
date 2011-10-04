using System;
using EnsureThat;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlInclude : ISqlInclude
    {
        public string Sql { get; private set; }

        public SqlInclude(string sql)
        {
            Ensure.That(() => sql).IsNotNullOrWhiteSpace();

            Sql = sql;
        }
    }
}