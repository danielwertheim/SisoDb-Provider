using System;
using EnsureThat;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlSorting
    {
        private readonly string _sorting;
        private readonly bool _isEmpty;

        public virtual string Sorting
        {
            get { return _sorting; }
        }

        public virtual bool IsEmpty
        {
            get { return _isEmpty; }
        }

        public SqlSorting(string sorting)
        {
            Ensure.That(sorting, "sorting").IsNotNullOrWhiteSpace();

            _isEmpty = false;
            _sorting = sorting;
        }

        protected SqlSorting()
        {
            _isEmpty = true;
            _sorting = string.Empty;
        }

        public static SqlSorting Empty()
        {
            return new SqlSorting();
        }
    }
}