using System;
using EnsureThat;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class SortingNode : INode
    {
        public string MemberPath { get; private set; }

        public Type MemberType { get; private set; }

        public SortDirections Direction { get; private set; }

        public SortingNode(string memberPath, Type memberType, SortDirections direction = SortDirections.Asc)
        {
            Ensure.That(memberPath,  "memberPath").IsNotNullOrWhiteSpace();
            Ensure.That(memberType, "memberType").IsNotNull();

            MemberPath = memberPath;
            MemberType = memberType;
            Direction = direction;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", MemberPath, Direction);
        }
    }
}