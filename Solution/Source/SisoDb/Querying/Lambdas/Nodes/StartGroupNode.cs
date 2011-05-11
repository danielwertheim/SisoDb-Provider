using System;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class StartGroupNode : INode
    {
        public override string ToString()
        {
            return "(";
        }
    }
}