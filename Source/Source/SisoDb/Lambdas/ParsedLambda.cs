using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SisoDb.Lambdas.Nodes;

namespace SisoDb.Lambdas
{
    [Serializable]
    public class ParsedLambda : IParsedLambda
    {
        public ReadOnlyCollection<INode> Nodes { get; private set; }

        public ParsedLambda(IEnumerable<INode> nodes)
        {
            nodes.AssertHasItems("nodes");

            Nodes = new ReadOnlyCollection<INode>(nodes.ToList());
        }

        public IParsedLambda MergeAsNew(IParsedLambda other)
        {
            var thisNodes = Nodes.ToList();
            var otherNodes = other.Nodes;

            return new ParsedLambda(thisNodes.Union(otherNodes));
        }
    }
}