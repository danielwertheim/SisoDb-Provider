using System;
using EnsureThat;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class ValueNode : INode
    {
        public object Value { get; private set; }

        public ValueNode(object value)
        {
            Ensure.That(value, "value").IsNotNull();

            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}