using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Lambdas.Operators;

namespace SisoDb.Querying.Lambdas.Parsers
{
    public class NullableNodeTransformer : INodeTransformer
    {
        public virtual INode[] Transform(int maxIndex, ref int index, INode node, INodesCollection orgNodes)
        {
            var nullableNode = node as NullableMemberNode;
            if (nullableNode == null || !nullableNode.IsForHasValueCheck)
                return new[] { node };

            if (index == maxIndex)
                return new INode[] { nullableNode, new OperatorNode(Operator.IsNot()), new NullNode() };

            var rightOperator = orgNodes.GetItemOrNullByIndex(index + 1) as OperatorNode;
            var rightValue = orgNodes.GetItemOrNullByIndex(index + 2) as ValueNode;

            if (rightOperator == null || rightValue == null)
                return new INode[] { nullableNode, new OperatorNode(Operator.IsNot()), new NullNode() };

            var operatorType = rightOperator.Operator.OperatorType;
            var hasValueIs = (bool)rightValue.Value;

            var shouldBeIsNotNullNodes =
                (operatorType == Operator.Types.Equal && hasValueIs)
                || (operatorType == Operator.Types.NotEqual && hasValueIs == false);

            var shouldBeIsNullNodes =
                (operatorType == Operator.Types.Equal && hasValueIs == false)
                || (operatorType == Operator.Types.Equal && hasValueIs == false);

            if (shouldBeIsNotNullNodes)
            {
                index += 2;
                return new INode[] { nullableNode, new OperatorNode(Operator.IsNot()), new NullNode() };
            }

            if (shouldBeIsNullNodes)
            {
                index += 2;
                return new INode[] { nullableNode, new OperatorNode(Operator.Is()), new NullNode() };
            }

            return new[] { node };
        }
    }
}