using System;
using EnsureThat;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlSortingMember
    {
        private readonly int _index;
        private readonly string _memberPath;
        private readonly string _indexStorageColumnName;
        private readonly string _direction;
        private readonly bool _isEmpty;
    	private readonly Type _dataType;

        public virtual int Index
        {
            get { return _index; }
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

    	public virtual Type DataType
    	{
			get { return _dataType; }
    	}

        public virtual bool IsEmpty
        {
            get { return _isEmpty; }
        }

        public SqlSortingMember(int index, string memberPath, string indexStorageColumnName, string direction, Type dataType)
        {
            Ensure.That(index, "index").IsGte(0);
            Ensure.That(memberPath, "memberPath").IsNotNullOrWhiteSpace();
            Ensure.That(indexStorageColumnName, "indexStorageColumnName").IsNotNullOrWhiteSpace();
            Ensure.That(direction, "direction").IsNotNullOrWhiteSpace();
        	Ensure.That(dataType, "dataType").IsNotNull();

            _isEmpty = false;
            _index = index;
            _memberPath = memberPath;
            _indexStorageColumnName = indexStorageColumnName;
            _direction = direction;
        	_dataType = dataType;
        }

        private SqlSortingMember()
        {
            _isEmpty = true;
            _index = -1;
            _memberPath = string.Empty;
            _indexStorageColumnName = string.Empty;
            _direction = string.Empty;
        	_dataType = TypeFor<object>.Type;
        }

        public static SqlSortingMember Empty()
        {
            return new SqlSortingMember();
        }
    }
}