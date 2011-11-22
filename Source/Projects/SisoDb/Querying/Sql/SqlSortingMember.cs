using System;
using EnsureThat;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlSortingMember
    {
        private readonly string _memberPath;
        private readonly string _alias;
        private readonly string _indexStorageColumnName;
        private readonly string _direction;
        private readonly bool _isEmpty;

        public virtual string Alias
        {
            get { return _alias; }
        }

        public virtual string MemberPath
        {
            get { return _memberPath; }
        }

        public virtual string IndexStorageColumnName
        {
            get { return _indexStorageColumnName; }
        }

        public virtual string Direction
        {
            get { return _direction; }
        }

        public virtual bool IsEmpty
        {
            get { return _isEmpty; }
        }

        public SqlSortingMember(string memberPath, string alias, string indexStorageColumnName, string direction)
        {
            Ensure.That(memberPath, "memberPath").IsNotNullOrWhiteSpace();
            Ensure.That(alias, "alias").IsNotNullOrWhiteSpace();
            Ensure.That(indexStorageColumnName, "indexStorageColumnName").IsNotNullOrWhiteSpace();
            Ensure.That(direction, "direction").IsNotNullOrWhiteSpace();

            _isEmpty = false;

            _memberPath = memberPath;
            _alias = alias;
            _indexStorageColumnName = indexStorageColumnName;
            _direction = direction;
        }

        private SqlSortingMember()
        {
            _isEmpty = true;
            _memberPath = string.Empty;
            _alias = string.Empty;
            _indexStorageColumnName = string.Empty;
            _direction = string.Empty;
        }

        public static SqlSortingMember Empty()
        {
            return new SqlSortingMember();
        }

        //public static string ToSortingString(IList<SqlSorting> sortings, string decorateSortingWith)
        //{
        //    var transformedSortings = new List<string>();

        //    foreach (var sorting in sortings)
        //    {
        //        if (sorting.IndexStorageColumnName == StructureIdColumnName)
        //            transformedSortings.Add(string.Format("{0} {1}", IndexStorageSchema.Fields.StructureId.Name, sorting.Direction));
        //        else
        //            transformedSortings.Add(string.Format("{0} {1}", string.Format(decorateSortingWith, sorting.Sorting), sorting.Direction));
        //    }

        //    return string.Join(", ", transformedSortings);
        //}

        //public static string ToColumnDefinitionString(IEnumerable<SqlSorting> sortings, string decorateSortingWith)
        //{
        //    var transformedSortings = new List<string>();

        //    foreach(var sorting in sortings)
        //    {
        //        if(sorting.IndexStorageColumnName == StructureIdColumnName)
        //            transformedSortings.Add(string.Format("{0} {1}", IndexStorageSchema.Fields.StructureId.Name, sorting.Alias));
        //        else
        //            transformedSortings.Add(string.Format("{0} {1}", string.Format(decorateSortingWith, sorting.IndexStorageColumnName), sorting.Alias));
        //    }

        //    return string.Join(", ", transformedSortings);
        //}

        //public static string ToAliasAndDirectionString(IEnumerable<SqlSorting> sortings)
        //{
        //    var transformedSortings = sortings.Select(s => string.Format("{0} {1}", s.Alias, s.Direction));

        //    return string.Join(", ", transformedSortings);
        //}
    }
}