using System;
using System.Collections;
using System.Collections.Generic;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class NodesContainer : IEnumerable<INode>
    {
        private readonly Queue<INode> _nodes;

        public NodesContainer()
        {
            _nodes = new Queue<INode>();
        }

        public void AddNode(INode node)
        {
            _nodes.Enqueue(node);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<INode> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }
    }
}