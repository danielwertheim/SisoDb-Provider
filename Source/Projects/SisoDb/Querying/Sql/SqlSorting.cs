using System;
using System.Linq;
using EnsureThat;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlSorting
    {
        public virtual string[] SortingJoins { get; private set; }

        public virtual string SortingSql { get; private set; }

        public virtual bool IsEmpty
        {
            get { return SortingJoins.Length < 1 && string.IsNullOrWhiteSpace(SortingSql); }
        }

        public SqlSorting(string[] sortingJoins, string sortingSql)
        {
            Ensure.That(sortingJoins, "sortingJoins").HasItems();
            Ensure.That(sortingSql, "sortingSql").IsNotNullOrWhiteSpace();

            SortingJoins = sortingJoins;
            SortingSql = sortingSql;
        }

        protected SqlSorting()
        {
            SortingJoins = Enumerable.Empty<string>().ToArray(); 
            SortingSql = string.Empty;
        }

        public static SqlSorting Empty()
        {
            return new SqlSorting();
        }
    }
}