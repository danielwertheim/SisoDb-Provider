using System;

namespace SisoDb.Lambdas.Nodes
{
    [Serializable]
    public class SortingNode : INode
    {
        public string MemberPath { get; private set; }

        public SortDirections Direction { get; private set; }

        public SortingNode(string memberPath, SortDirections direction = SortDirections.Asc)
        {
            MemberPath = memberPath.AssertNotNull("memberPath");
            Direction = direction;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", MemberPath, Direction);
        }
    }
}