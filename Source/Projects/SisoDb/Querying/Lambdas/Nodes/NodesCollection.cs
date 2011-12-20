using System;
using System.Collections;
using System.Collections.Generic;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class NodesCollection : INodesCollection
    {
        private readonly List<INode> _nodes;

        public NodesCollection()
        {
            _nodes = new List<INode>();
        }

    	public INode this[int index]
    	{
			get { return _nodes[index]; }
    	}

    	public int Count
    	{
    		get { return _nodes.Count; }
    	}

    	public void AddNode(INode node)
        {
            _nodes.Add(node);
        }

    	public void AddNodes(params INode[] nodes)
    	{
    		_nodes.AddRange(nodes);
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