using System.Collections.Generic;
using System.Linq.Expressions;

namespace SisoDb.Lambdas.Nodes
{
    internal class NodesContainer
    {
        private readonly Queue<INode> _nodes;

        public LambdaExpression Expression { get; private set; }

        public IList<INode> Nodes
        {
            get { return _nodes.ToArray(); }
        }

        public NodesContainer(LambdaExpression e)
        {
            Expression = e;
            _nodes = new Queue<INode>();
        }

        public void AddNode(INode node)
        {
            _nodes.Enqueue(node);
        }
    }
}