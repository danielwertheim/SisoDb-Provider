namespace SisoDb.Lambdas.Nodes
{
    internal class ValueNode : INode
    {
        public object Value { get; private set; }

        public ValueNode(object value)
        {
            Value = value.AssertNotNull("value");
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}