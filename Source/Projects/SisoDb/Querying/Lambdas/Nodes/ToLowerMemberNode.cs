using System;
using SisoDb.Structures;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class ToLowerMemberNode : MemberNode, IStringOperationMemberNode
    {
        public ToLowerMemberNode(string memberPath, Type dataType, DataTypeCode dataTypeCode)
            : base(memberPath, dataType, dataTypeCode) {}
    }
}