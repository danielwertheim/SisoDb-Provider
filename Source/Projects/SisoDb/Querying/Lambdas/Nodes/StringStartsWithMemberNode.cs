using System;
using SisoDb.PineCone.Structures;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
	public class StringStartsWithMemberNode : MemberNode, IStringOperationMemberNode
	{
        public string Value { get; private set; }

	    public StringStartsWithMemberNode(string memberPath, Type dataType, DataTypeCode dataTypeCode, string value)
			: base(memberPath, dataType, dataTypeCode)
	    {
	        Value = value;
	    }
	}
}