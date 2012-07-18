using System;
using PineCone.Structures;
using SisoDb.EnsureThat;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class SortingNode : INode
    {
        public string MemberPath { get; private set; }

        public Type DataType { get; private set; }

        public DataTypeCode DataTypeCode { get; private set; }

        public SortDirections Direction { get; private set; }

        public SortingNode(string memberPath, Type dataType, DataTypeCode dataTypeCode, SortDirections direction = SortDirections.Asc)
        {
            Ensure.That(memberPath,  "memberPath").IsNotNullOrWhiteSpace();
            Ensure.That(dataType, "dataType").IsNotNull();

            MemberPath = memberPath;
            DataType = dataType;
            DataTypeCode = dataTypeCode;
            Direction = direction;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", MemberPath, Direction);
        }
    }
}