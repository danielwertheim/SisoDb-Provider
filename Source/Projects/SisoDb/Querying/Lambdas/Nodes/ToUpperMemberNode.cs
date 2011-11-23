using System;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class ToUpperMemberNode : MemberNode
    {
        public ToUpperMemberNode(string memberPath, Type memberType)
            : base(memberPath, memberType) { }
    }
}