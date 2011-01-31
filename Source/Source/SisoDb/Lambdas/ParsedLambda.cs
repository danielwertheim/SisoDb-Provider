using System.Collections.Generic;
using System.Linq.Expressions;
using SisoDb.Lambdas.Nodes;

namespace SisoDb.Lambdas
{
    internal class ParsedLambda : IParsedLambda
    {
        public LambdaExpression Expression { get; private set; }
        
        public IList<INode> Nodes { get; private set; }

        public ParsedLambda(LambdaExpression e, IEnumerable<INode> nodes)
        {
            Expression = e;
            Nodes = new List<INode>(nodes);
        }
    }
}