using System;
using PineCone.Structures;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class StringExactMemberNode : MemberNode, IStringOperationMemberNode
    {
        public string Value { get; private set; }

        public StringExactMemberNode(string memberPath, Type dataType, DataTypeCode dataTypeCode, string value)
            : base(memberPath, dataType, dataTypeCode)
        {
            Value = value;
        }
    }
}