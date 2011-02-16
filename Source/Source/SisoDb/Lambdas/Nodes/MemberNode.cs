using System.Linq.Expressions;

namespace SisoDb.Lambdas.Nodes
{
    public class MemberNode : INode
    {
        private static readonly NameStrategy NameStrategy = new NameStrategy();

        public MemberNode Parent { get; private set; }

        public MemberExpression Expression { get; private set; }

        public string Name { get; private set; }

        public string Path { get; private set;  }

        public bool IsElementOfEnumerable { get; private set; }

        public MemberNode(MemberNode parent, bool isElementOfEnumerable, MemberExpression e)
        {
            Parent = parent;
            Expression = e;
            Name = NameStrategy.Apply(e.Member.Name.AssertNotNull("name"));
            Path = Parent == null ? Name : Parent.Path + "." + Name;
            IsElementOfEnumerable = isElementOfEnumerable;
        }

        public override string ToString()
        {
            return Path;
        }
    }
}