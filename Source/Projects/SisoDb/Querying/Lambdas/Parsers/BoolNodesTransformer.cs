using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Lambdas.Operators;

namespace SisoDb.Querying.Lambdas.Parsers
{
	public class BoolNodesTransformer : INodesTransformer
	{
		public INodesCollection Transform(INodesCollection nodes)
		{
			var newNodes = new NodesCollection();
			var boolType = typeof (bool);

			for (var i = 0; i < nodes.Count; i++)
			{
				var memberNode = nodes[i] as MemberNode;
				if(memberNode == null || memberNode.MemberType != boolType)
				{
					newNodes.AddNode(nodes[i]);
					continue;
				}

				var firstNextNode = nodes.GetItemOrNullByIndex(i + 1) as OperatorNode;
				var secondNextNode = nodes.GetItemOrNullByIndex(i + 2) as ValueNode;
				var memberNodeIsPartOfCompleteBoolExpression = 
					firstNextNode != null && 
					secondNextNode != null &&
					secondNextNode.Value is bool;
				
				if (memberNodeIsPartOfCompleteBoolExpression)
					newNodes.AddNode(memberNode);
				else
					newNodes.AddNodes(memberNode, new OperatorNode(Operator.Equal()), new ValueNode(true));
			}

			return newNodes;
		}
	}
}