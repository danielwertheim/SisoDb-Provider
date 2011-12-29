using System;

namespace SisoDb.Querying.Lambdas.Nodes
{
	[Serializable]
	public class StartsWithMemberNode : MemberNode
	{
		public StartsWithMemberNode(string memberPath, Type memberType)
			: base(memberPath, memberType) {}
	}
}