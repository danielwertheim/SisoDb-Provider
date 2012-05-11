using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Lambdas.Operators;
using NCore.Reflections;

namespace SisoDb.Querying.Lambdas.Parsers
{
    public class BoolNodeTransformer : INodeTransformer
    {
        public virtual INode[] Transform(int maxIndex, ref int index, INode node, INodesCollection orgNodes)
        {
            var memberNode = node as MemberNode;
            if (memberNode == null || !memberNode.MemberType.IsBoolType())
                return new[] { node };

            var firstNextNode = orgNodes.GetItemOrNullByIndex(index + 1) as OperatorNode;
            var secondNextNode = orgNodes.GetItemOrNullByIndex(index + 2) as ValueNode;
            var memberNodeIsPartOfCompleteBoolExpression =
                firstNextNode != null &&
                secondNextNode != null &&
                secondNextNode.Value is bool;

            return memberNodeIsPartOfCompleteBoolExpression 
                ? new[] { memberNode } 
                : new INode[] { memberNode, new OperatorNode(Operator.Equal()), new ValueNode(true) };
        }
    }
}