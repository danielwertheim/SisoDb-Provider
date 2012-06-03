using System;
using PineCone.Structures;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class StringEqualsMemberNode : MemberNode
    {
        public string Value { get; set; }

        public bool ExactMatch { get; set; }

        public bool IsTextType { get; set; }

        public StringEqualsMemberNode(string memberPath, Type dataType, DataTypeCode dataTypeCode, bool isTextType, string value, bool exactMatch)
            : base(memberPath, dataType, dataTypeCode)
        {
            Value = value;
            ExactMatch = exactMatch;
            IsTextType = isTextType;
        }
    }
}