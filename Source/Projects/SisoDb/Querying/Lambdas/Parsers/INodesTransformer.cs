using SisoDb.Querying.Lambdas.Nodes;

namespace SisoDb.Querying.Lambdas.Parsers
{
	public interface INodesTransformer
	{
		INodesCollection Transform(INodesCollection nodes);
	}
}