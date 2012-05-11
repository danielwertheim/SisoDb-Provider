using SisoDb.Querying.Lambdas.Nodes;

namespace SisoDb.Querying.Lambdas.Parsers
{
	public interface INodeTransformer
	{
		INode[] Transform(int maxIndex, ref int index, INode node, INodesCollection orgNodes);
	}
}