using System;
using EnsureThat;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlSorting
    {
        private readonly string _sortingJoins;
        private readonly string _sorting;
        private readonly bool _isEmpty;

        public virtual string SortingJoins
        {
            get { return _sortingJoins; }
        }

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
            _sortingJoins = string.Empty;
            _sorting = sorting;
        }

        public SqlSorting(string sorting, string sortingJoins)
        {
            Ensure.That(sorting, "sorting").IsNotNullOrWhiteSpace();
            Ensure.That(sortingJoins, "sortingJoins").IsNotNullOrWhiteSpace();

            _isEmpty = false;
            _sortingJoins = sortingJoins;
            _sorting = sorting;
        }

        protected SqlSorting()
        {
            _isEmpty = true;
            _sortingJoins = string.Empty; 
            _sorting = string.Empty;
        }

        public static SqlSorting Empty()
        {
            return new SqlSorting();
        }
    }
}