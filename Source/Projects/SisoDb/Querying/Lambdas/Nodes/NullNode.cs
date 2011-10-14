using System;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class NullNode : INode
    {
        public override string ToString()
        {
            return "null";
        }
    }
}