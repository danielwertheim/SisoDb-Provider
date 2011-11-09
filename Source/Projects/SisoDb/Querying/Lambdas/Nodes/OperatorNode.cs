using System;
using SisoDb.Querying.Lambdas.Operators;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class OperatorNode : INode
    {
        public Operator Operator { get; private set; }

        public OperatorNode(Operator op)
        {
            Operator = op;
        }

        public override string ToString()
        {
            return Operator.ToString();
        }
    }
}