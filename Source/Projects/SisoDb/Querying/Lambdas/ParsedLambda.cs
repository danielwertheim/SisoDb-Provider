using System;
using System.Linq;
using EnsureThat;
using SisoDb.Querying.Lambdas.Nodes;

namespace SisoDb.Querying.Lambdas
{
    [Serializable]
    public class ParsedLambda : IParsedLambda
    {
        public INode[] Nodes { get; private set; }

		public static IParsedLambda Empty()
		{
			return new ParsedLambda();
		}

    	private ParsedLambda()
    	{
    		Nodes = new INode[]{};
    	}

        public ParsedLambda(INode[] nodes)
        {
            Ensure.That(nodes, "nodes").HasItems();

        	Nodes = nodes;
        }

        public IParsedLambda MergeAsNew(IParsedLambda other)
        {
            return new ParsedLambda(Nodes.Union(other.Nodes).ToArray());
        }
    }
}