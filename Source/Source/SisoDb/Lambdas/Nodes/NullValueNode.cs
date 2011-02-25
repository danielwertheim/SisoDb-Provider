using System;

namespace SisoDb.Lambdas.Nodes
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