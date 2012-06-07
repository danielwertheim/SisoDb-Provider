using System;
using EnsureThat;
using PineCone.Structures;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class MemberNode : IMemberNode
    {
        public string Path { get; private set;  }
        public Type DataType { get; private set; }
        public DataTypeCode DataTypeCode { get; private set; }

        public MemberNode(string memberPath, Type dataType, DataTypeCode dataTypeCode)
        {
            Ensure.That(memberPath, "memberPath").IsNotNullOrWhiteSpace();
            Ensure.That(dataType, "dataType").IsNotNull();

            Path = memberPath;
            DataType = dataType;
            DataTypeCode = dataTypeCode;
        }

        public override string ToString()
        {
            return Path;
        }
    }
}