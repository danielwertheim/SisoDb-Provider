using System;
using SisoDb.EnsureThat;
using SisoDb.PineCone.Structures;

namespace SisoDb.Dac
{
    [Serializable]
    public class ArrayDacParameter : DacParameter
    {
        public Type MemberDataType { get; private set; }
        public DataTypeCode MemberDataTypeCode { get; private set; }

        public ArrayDacParameter(string name, Type memberDataType, DataTypeCode memberDataTypeCode, object[] value) : base(name, value)
        {
            Ensure.That(memberDataType, "memberDataType").IsNotNull();

            MemberDataType = memberDataType;
            MemberDataTypeCode = memberDataTypeCode;
        }
    }
}