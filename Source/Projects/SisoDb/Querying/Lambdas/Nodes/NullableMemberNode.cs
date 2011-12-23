using System;
using NCore;
using NCore.Reflections;

namespace SisoDb.Querying.Lambdas.Nodes
{
	[Serializable]
	public class NullableMemberNode : MemberNode
	{
		public bool IsForHasValueCheck { get; private set; }

		public NullableMemberNode(string memberPath, Type memberType) : base(ApplyPathRules(memberPath), ExtractType(memberType))
		{
			IsForHasValueCheck = memberPath.EndsWith(".HasValue");
		}

		protected static string ApplyPathRules(string path)
		{
			return path.EndsWithAny(".Value", ".HasValue")
				? path.Substring(0, path.LastIndexOf('.'))
				: path;
		}

		protected static Type ExtractType(Type memberType)
		{
			if (memberType.IsNullablePrimitiveType())
				return memberType.GetGenericArguments()[0];

			return memberType;
		}
	}
}