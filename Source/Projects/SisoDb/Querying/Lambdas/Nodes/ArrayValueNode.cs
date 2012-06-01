using System;
using EnsureThat;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class ArrayValueNode : INode
    {
        public object[] Value { get; private set; }

        public ArrayValueNode(object[] value)
        {
            Ensure.That(value, "value").HasItems();

            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}