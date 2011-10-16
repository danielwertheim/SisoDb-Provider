using System;
using EnsureThat;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlInclude
    {
        public virtual string Sql { get; private set; }

        public virtual bool IsEmpty
        {
            get { return string.IsNullOrWhiteSpace(Sql); }
        }

        public SqlInclude(string sql)
        {
            Ensure.That(sql, "sql").IsNotNullOrWhiteSpace();

            Sql = sql;
        }

        protected SqlInclude()
        {
            Sql = string.Empty;
        }

        public static SqlInclude Empty()
        {
            return new SqlInclude();
        }
    }
}