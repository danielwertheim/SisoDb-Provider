using System;
using System.Linq;
using SisoDb.EnsureThat;
using SisoDb.Querying.Lambdas.Nodes;

namespace SisoDb.Querying.Lambdas
{
    [Serializable]
    public class ParsedLambda : IParsedLambda
    {
        private readonly string _asString;

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
            _asString = Nodes.AsString();
        }

        public IParsedLambda MergeAsNew(IParsedLambda other)
        {
            return new ParsedLambda(Nodes.Union(other.Nodes).ToArray());
        }

        public string AsString()
        {
            return _asString;
        }

        public override string ToString()
        {
            return AsString();
        }
    }
}