using System;
using System.Linq.Expressions;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class MemberNode : INode
    {
        public MemberNode Parent { get; private set; }
        
        public string Name { get; private set; }

        public string Path { get; private set;  }

        public bool IsElementOfEnumerable { get; private set; }

        public Type MemberType { get; private set; }

        public MemberNode(MemberNode parent, bool isElementOfEnumerable, MemberExpression e)
        {
            Parent = parent;
            Name = e.Member.Name;
            Path = Parent == null ? Name : Parent.Path + "." + Name;
            IsElementOfEnumerable = isElementOfEnumerable;
            MemberType = e.Type;
        }

        public override string ToString()
        {
            return Path;
        }
    }
}