using System;

namespace SisoDb.Querying.Lambdas.Nodes
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