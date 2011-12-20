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

				var nextNode = nodes[i];
				if (!(nextNode is OperatorNode))
					newNodes.AddNodes(memberNode, new OperatorNode(Operator.Equal()), new ValueNode(true));
			}

			return newNodes;
		}
	}
}