using System;
using SisoDb.PineCone.Structures;

namespace SisoDb.Querying.Lambdas.Nodes
{
    public interface IMemberNode : INode
    {
        string Path { get; }
        Type DataType { get; }
        DataTypeCode DataTypeCode { get; }
    }
}