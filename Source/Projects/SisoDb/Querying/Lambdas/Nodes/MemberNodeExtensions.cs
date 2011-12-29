namespace SisoDb.Querying.Lambdas.Nodes
{
	public static class MemberNodeExtensions
	{
		public static StartsWithMemberNode ToStartsWithNode(this MemberNode memberNode)
		{
			return new StartsWithMemberNode(memberNode.Path, memberNode.MemberType);
		}

		public static EndsWithMemberNode ToEndsWithNode(this MemberNode memberNode)
		{
			return new EndsWithMemberNode(memberNode.Path, memberNode.MemberType);
		}

		public static ToLowerMemberNode ToLowerNode(this MemberNode memberNode)
		{
			return new ToLowerMemberNode(memberNode.Path, memberNode.MemberType);
		}

		public static ToUpperMemberNode ToUpperNode(this MemberNode memberNode)
		{
			return new ToUpperMemberNode(memberNode.Path, memberNode.MemberType);
		}
	}
}