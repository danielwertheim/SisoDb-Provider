using System;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class NullValueNode : INode
    {
        public override string ToString()
        {
            return "null";
        }
    }
}