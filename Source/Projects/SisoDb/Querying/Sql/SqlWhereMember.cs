using System;
using PineCone.Structures;
using SisoDb.EnsureThat;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlWhereMember
    {
        private readonly int _index;
        private readonly string _memberPath;
        private readonly string _alias;
        private readonly bool _isEmpty;
    	private readonly Type _dataType;
        private readonly DataTypeCode _dataTypeCode;

    	public virtual int Index
        {
            get { return _index; }
        }

        public virtual string MemberPath
        {
            get { return _memberPath; }
        }

        public virtual string Alias
        {
            get { return _alias; }
        }

    	public virtual Type DataType
    	{
			get { return _dataType; }
    	}

        public virtual DataTypeCode DataTypeCode
        {
            get { return _dataTypeCode; }
        }

        public virtual bool IsEmpty
        {
            get { return _isEmpty; }
        }

        public SqlWhereMember(int index, string memberPath, Type dataType, DataTypeCode dataTypeCode)
        {
            Ensure.That(index, "index").IsGte(0);
            Ensure.That(memberPath, "memberPath").IsNotNullOrWhiteSpace();
        	Ensure.That(dataType, "dataType").IsNotNull();

            _isEmpty = false;
            _index = index;
            _memberPath = memberPath;
            _alias = string.Concat("mem", _index);
        	_dataType = dataType;
            _dataTypeCode = dataTypeCode;
        }

        private SqlWhereMember()
        {
            _isEmpty = true;
            _index = -1;
            _memberPath = string.Empty;
            _alias = string.Empty;
        	_dataType = TypeFor<object>.Type;
            _dataTypeCode = DataTypeCode.Unknown;
        }

        public static SqlWhereMember Empty()
        {
            return new SqlWhereMember();
        }
    }
}