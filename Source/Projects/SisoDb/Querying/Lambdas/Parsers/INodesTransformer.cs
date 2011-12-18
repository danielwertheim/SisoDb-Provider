using SisoDb.Querying.Lambdas.Nodes;

namespace SisoDb.Querying.Lambdas.Parsers
{
	public interface INodesTransformer
	{
		INodes Transform(INodes nodes);
	}
}