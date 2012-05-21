using System;
using PineCone.Structures;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class ToLowerMemberNode : MemberNode
    {
        public ToLowerMemberNode(string memberPath, Type dataType, DataTypeCode dataTypeCode)
            : base(memberPath, dataType, dataTypeCode) {}
    }
}