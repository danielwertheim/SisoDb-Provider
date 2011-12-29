using System;

namespace SisoDb.Querying.Lambdas.Nodes
{
	[Serializable]
	public class EndsWithMemberNode : MemberNode
	{
		public EndsWithMemberNode(string memberPath, Type memberType)
			: base(memberPath, memberType) {}
	}
}