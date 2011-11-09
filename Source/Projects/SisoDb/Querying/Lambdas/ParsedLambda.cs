using System;
using System.Collections.ObjectModel;
using System.Linq;
using EnsureThat;
using SisoDb.Querying.Lambdas.Nodes;

namespace SisoDb.Querying.Lambdas
{
    [Serializable]
    public class ParsedLambda : IParsedLambda
    {
        public ReadOnlyCollection<INode> Nodes { get; private set; }

        public ParsedLambda(INode[] nodes)
        {
            Ensure.That(nodes, "nodes").HasItems();

            Nodes = new ReadOnlyCollection<INode>(nodes.ToList());
        }

        public IParsedLambda MergeAsNew(IParsedLambda other)
        {
            var thisNodes = Nodes.ToList();
            var otherNodes = other.Nodes;

            return new ParsedLambda(thisNodes.Union(otherNodes).ToArray()); //TODO: Union or Merge?
        }
    }
}