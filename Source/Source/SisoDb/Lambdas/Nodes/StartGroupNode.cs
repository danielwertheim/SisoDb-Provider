using System;

namespace SisoDb.Lambdas.Nodes
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