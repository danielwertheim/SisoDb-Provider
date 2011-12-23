using System.Linq;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Lambdas.Operators;

namespace SisoDb.Querying.Lambdas.Parsers
{
	public class NullableNodesTransformer : INodesTransformer
	{
		public INodesCollection Transform(INodesCollection nodes)
		{
			if (!nodes.OfType<NullableMemberNode>().Any(n => n.IsForHasValueCheck))
				return nodes;

			var maxIndex = nodes.Count - 1;
			var newNodes = new NodesCollection();

			for (var i = 0; i < nodes.Count; i++)
			{
				var nullableNode = nodes[i] as NullableMemberNode;

				if (nullableNode == null || !nullableNode.IsForHasValueCheck)
				{
					newNodes.AddNode(nodes[i]);
					continue;
				}

				if (i == maxIndex)
				{
					newNodes.AddNodes(nullableNode, new OperatorNode(Operator.IsNot()), new NullNode());
					continue;
				}

				var rightOperator = nodes[i + 1] as OperatorNode;
				var rightValue = nodes[i + 2] as ValueNode;

				if (rightOperator == null || rightValue == null)
				{
					newNodes.AddNodes(nullableNode, new OperatorNode(Operator.IsNot()), new NullNode());
					continue;
				}

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
					newNodes.AddNodes(nullableNode, new OperatorNode(Operator.IsNot()), new NullNode());
					i += 2;
					continue;
				}

				if (shouldBeIsNullNodes)
				{
					newNodes.AddNodes(nullableNode, new OperatorNode(Operator.Is()), new NullNode());
					i += 2;
					continue;
				}
			}

			return newNodes;
		}
	}
}