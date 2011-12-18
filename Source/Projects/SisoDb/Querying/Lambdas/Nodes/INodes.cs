using System.Collections.Generic;

namespace SisoDb.Querying.Lambdas.Nodes
{
	public interface INodes : IEnumerable<INode>
	{
		INode this[int index] { get; }

		int Count { get; }

		void AddNode(INode node);

		void AddNodes(params INode[] nodes);
	}
}