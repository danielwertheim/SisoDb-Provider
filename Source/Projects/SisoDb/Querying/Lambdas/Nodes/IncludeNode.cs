using System;
using EnsureThat;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class IncludeNode : INode
    {
        public string ChildStructureName { get; private set; }

        public string IdReferencePath { get; private set; }

        public string ObjectReferencePath { get; private set; }

        public Type MemberType { get; private set; }

        public IncludeNode(string childStructureName, string idReferencePath, string objectReferencePath, Type memberType)
        {
            Ensure.That(childStructureName, "childStructureName").IsNotNullOrWhiteSpace();
            Ensure.That(idReferencePath, "idReferencePath").IsNotNullOrWhiteSpace();
            Ensure.That(objectReferencePath, "objectReferencePath").IsNotNullOrWhiteSpace();
            Ensure.That(memberType, "memberType").IsNotNull();

            ChildStructureName = childStructureName;
            IdReferencePath = idReferencePath;
            ObjectReferencePath = objectReferencePath;
            MemberType = memberType;
        }
    }
}