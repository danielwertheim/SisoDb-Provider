using System;
using EnsureThat;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlSorting
    {
        public virtual string SortingJoins { get; private set; }

        public virtual string Sorting { get; private set; }

        public virtual bool IsEmpty
        {
            get { return string.IsNullOrWhiteSpace(SortingJoins) && string.IsNullOrWhiteSpace(Sorting); }
        }

        public SqlSorting(string sorting)
        {
            Ensure.That(sorting, "sorting").IsNotNullOrWhiteSpace();

            SortingJoins = string.Empty;
            Sorting = sorting;
        }

        public SqlSorting(string sorting, string sortingJoins)
        {
            Ensure.That(sorting, "sorting").IsNotNullOrWhiteSpace();
            Ensure.That(sortingJoins, "sortingJoins").IsNotNullOrWhiteSpace();

            SortingJoins = sortingJoins;
            Sorting = sorting;
        }

        protected SqlSorting()
        {
            SortingJoins = string.Empty; 
            Sorting = string.Empty;
        }

        public static SqlSorting Empty()
        {
            return new SqlSorting();
        }
    }
}