using System;
using EnsureThat;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class MemberNode : INode
    {
        public string Path { get; private set;  }

        public Type MemberType { get; private set; }

        public MemberNode(string memberPath, Type memberType)
        {
            Ensure.That(memberPath, "memberPath").IsNotNullOrWhiteSpace();
            Ensure.That(memberType, "memberType").IsNotNull();

            Path = memberPath;
            MemberType = memberType;
        }

        public override string ToString()
        {
            return Path;
        }

        public ToLowerMemberNode AsToLowerNode()
        {
            return new ToLowerMemberNode(Path, MemberType);
        }

        public ToUpperMemberNode AsToUpperNode()
        {
            return new ToUpperMemberNode(Path, MemberType);
        }
    }
}