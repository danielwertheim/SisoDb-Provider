using System;
using SisoDb.Structures;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class NotInSetMemberNode : MemberNode
    {
        public object[] Values { get; set; }

        public NotInSetMemberNode(string memberPath, Type dataType, DataTypeCode dataTypeCode, object[] values)
            : base(memberPath, dataType, dataTypeCode)
        {
            Values = values;
        }
    }
}