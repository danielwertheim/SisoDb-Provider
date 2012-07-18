using System;
using PineCone.Structures;
using SisoDb.EnsureThat;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class ArrayValueNode : INode
    {
        public Type MemberDataType { get; private set; }
        public DataTypeCode MemberDataTypeCode { get; private set; }
        public object[] Value { get; private set; }

        public ArrayValueNode(Type memberDataType, DataTypeCode memberDataTypeCode, object[] value)
        {
            Ensure.That(memberDataType, "memberDataType").IsNotNull();
            Ensure.That(value, "value").HasItems();

            MemberDataType = memberDataType;
            MemberDataTypeCode = memberDataTypeCode;
            Value = value;
        }
    }
}