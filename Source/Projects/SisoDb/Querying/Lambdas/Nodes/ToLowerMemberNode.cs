using System;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class ToLowerMemberNode : MemberNode
    {
        public ToLowerMemberNode(string memberPath, Type memberType)
            : base(memberPath, memberType) {}
    }
}