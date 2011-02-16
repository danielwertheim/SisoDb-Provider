using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SisoDb.Lambdas.Nodes;

namespace SisoDb.Lambdas
{
    public class ParsedLambda : IParsedLambda
    {
        public ReadOnlyCollection<INode> Nodes { get; private set; }

        public ParsedLambda(IEnumerable<INode> nodes)
        {
            nodes.AssertHasItems("nodes");

            Nodes = new ReadOnlyCollection<INode>(nodes.ToList());
        }
    }
}