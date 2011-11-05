using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using SisoDb.DbSchema;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlSorting
    {
        private readonly static string StructureIdColumnName = string.Format("[{0}]", StructureStorageSchema.Fields.Id.Name);

        private readonly string _sorting;
        private readonly string _columnName;
        private readonly string _alias;
        private readonly string _direction;
        private readonly bool _isEmpty;
        
        public virtual string Sorting
        {
            get { return _sorting; }
        }

        public virtual string ColumnName
        {
            get { return _columnName; }
        }

        public virtual string Alias
        {
            get { return _alias; }
        }

        public virtual string Direction
        {
            get { return _direction; }
        }

        public virtual bool IsEmpty
        {
            get { return _isEmpty; }
        }

        public SqlSorting(string sorting, string columnName, string alias, string direction)
        {
            Ensure.That(sorting, "sorting").IsNotNullOrWhiteSpace();
            Ensure.That(columnName, "columnName").IsNotNullOrWhiteSpace();
            Ensure.That(alias, "alias").IsNotNullOrWhiteSpace();
            Ensure.That(direction, "direction").IsNotNullOrWhiteSpace();

            _isEmpty = false;
            _sorting = sorting;
            _columnName = columnName;
            _alias = alias;
            _direction = direction;
        }

        protected SqlSorting()
        {
            _isEmpty = true;
            _sorting = string.Empty;
            _columnName = string.Empty;
            _alias = string.Empty;
            _direction = string.Empty;
        }

        public static SqlSorting Empty()
        {
            return new SqlSorting();
        }

        public static string ToColumnDefinitionString(IEnumerable<SqlSorting> sortings, string decorateSortingWith)
        {
            var transformedSortings = new List<string>();

            foreach(var sorting in sortings)
            {
                if(sorting.ColumnName == StructureIdColumnName)
                    transformedSortings.Add(string.Format("{0} {1}", sorting.Sorting, sorting.Alias));
                else
                    transformedSortings.Add(string.Format("{0} {1}", string.Format(decorateSortingWith, sorting.Sorting), sorting.Alias));
            }

            return string.Join(", ", transformedSortings);
        }

        public static string ToSortingString(IEnumerable<SqlSorting> sortings, string decorateSortingWith)
        {
            var transformedSortings = new List<string>();

            foreach (var sorting in sortings)
            {
                if (sorting.ColumnName == StructureIdColumnName)
                    transformedSortings.Add(string.Format("{0} {1}", sorting.Sorting, sorting.Direction));
                else
                    transformedSortings.Add(string.Format("{0} {1}", string.Format(decorateSortingWith, sorting.Sorting), sorting.Direction));
            }

            return string.Join(", ", transformedSortings);
        }

        public static string ToAliasAndDirectionString(IEnumerable<SqlSorting> sortings)
        {
            var transformedSortings = sortings.Select(s => string.Format("{0} {1}", s.Alias, s.Direction));

            return string.Join(", ", transformedSortings);
        }
    }
}