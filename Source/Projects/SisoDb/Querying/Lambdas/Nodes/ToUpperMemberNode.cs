using System;
using PineCone.Structures;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class ToUpperMemberNode : MemberNode
    {
        public ToUpperMemberNode(string memberPath, Type dataType, DataTypeCode dataTypeCode)
            : base(memberPath, dataType, dataTypeCode) { }
    }
}