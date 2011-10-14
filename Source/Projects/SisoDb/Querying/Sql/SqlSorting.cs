using System;
using EnsureThat;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlSorting
    {
        public virtual string Sql { get; private set; }

        public virtual bool IsEmpty
        {
            get { return string.IsNullOrWhiteSpace(Sql); }
        }

        public SqlSorting(string sql)
        {
            Ensure.That(() => sql).IsNotNullOrWhiteSpace();
            Sql = sql;
        }

        protected SqlSorting()
        {
            Sql = string.Empty;
        }

        public static SqlSorting Empty()
        {
            return new SqlSorting();
        }

        public override string ToString()
        {
            return Sql;
        }
    }
}