using System;
using EnsureThat;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlInclude
    {
        private readonly int _index;
        private readonly string _tableName;
        private readonly string _indexValueColumnName;
        private readonly string _memberPathReference;
        private readonly string _objectReferencePath;
        private readonly bool _isEmpty;

        public virtual int Index
        {
            get { return _index; }
        }

        public virtual string TableName
        {
            get { return _tableName; }
        }

        public virtual string IndexValueColumnName
        {
            get { return _indexValueColumnName; }
        }

        public virtual string MemberPathReference
        {
            get { return _memberPathReference; }
        }

        public virtual string ObjectReferencePath
        {
            get { return _objectReferencePath; }
        }

        public virtual bool IsEmpty
        {
            get { return _isEmpty; }
        }

        public SqlInclude(int index, string tableName, string indexValueColumnName, string memberPathReference, string objectReferencePath)
        {
            Ensure.That(index, "index").IsGte(0);
            Ensure.That(tableName, "tableName").IsNotNullOrWhiteSpace();
            Ensure.That(indexValueColumnName, "indexValueColumnName").IsNotNullOrWhiteSpace();
            Ensure.That(memberPathReference, "memberPathReference").IsNotNullOrWhiteSpace();
            Ensure.That(objectReferencePath, "objectReferencePath").IsNotNullOrWhiteSpace();

            _isEmpty = false;
            _index = index;
            _tableName = tableName;
            _indexValueColumnName = indexValueColumnName;
            _memberPathReference = memberPathReference;
            _objectReferencePath = objectReferencePath;
        }

        private SqlInclude()
        {
            _isEmpty = true;
            _tableName = string.Empty;
            _indexValueColumnName = string.Empty;
            _index = -1;
            _memberPathReference = string.Empty;
            _objectReferencePath = string.Empty;
        }

        public static SqlInclude Empty()
        {
            return new SqlInclude();
        }
    }
}