using System;
using System.Linq.Expressions;
using SisoDb.Core;
using SisoDb.Core.Expressions;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class MemberNode : INode
    {
        public string Name { get; private set; }

        public string Path { get; private set;  }

        public Type MemberType { get; private set; }

        public MemberNode(MemberNode parent, MemberExpression member)
        {
            member.AssertNotNull("member");

            Name = member.Member.Name;
            Path = parent == null ? member.Path() : parent.Path + "." + member.Path();
            MemberType = member.Type;
        }

        public override string ToString()
        {
            return Path;
        }
    }
}