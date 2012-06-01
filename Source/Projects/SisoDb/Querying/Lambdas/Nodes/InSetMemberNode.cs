using System;
using PineCone.Structures;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class InSetMemberNode : MemberNode
    {
        public object[] Values { get; set; }

        public InSetMemberNode(string memberPath, Type dataType, DataTypeCode dataTypeCode, object[] values)
            : base(memberPath, dataType, dataTypeCode)
        {
            Values = values;
        }
    }
}