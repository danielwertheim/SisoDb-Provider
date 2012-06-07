using System;
using PineCone.Structures;

namespace SisoDb.Querying.Lambdas.Nodes
{
	[Serializable]
    public class StringEndsWithMemberNode : MemberNode, IStringOperationMemberNode
	{
	    public string Value { get; private set; }

	    public StringEndsWithMemberNode(string memberPath, Type dataType, DataTypeCode dataTypeCode, string value)
			: base(memberPath, dataType, dataTypeCode)
		{
		    Value = value;
		}
	}
}