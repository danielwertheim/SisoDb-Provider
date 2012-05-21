using System;
using PineCone.Structures;

namespace SisoDb.Querying.Lambdas.Nodes
{
	[Serializable]
	public class EndsWithMemberNode : MemberNode
	{
		public EndsWithMemberNode(string memberPath, Type dataType, DataTypeCode dataTypeCode)
			: base(memberPath, dataType, dataTypeCode) {}
	}
}