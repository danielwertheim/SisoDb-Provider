using System;

namespace SisoDb.Lambdas.Nodes
{
    [Serializable]
    public class EndGroupNode : INode
    {
        public override string ToString()
        {
            return ")";
        }
    }
}