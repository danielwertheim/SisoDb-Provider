using System;
using SisoDb.Core;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class IncludeNode : INode
    {
        public string ChildStructureName { get; private set; }

        public string IdReferencePath { get; private set; }

        public string ObjectReferencePath { get; private set; }

        public IncludeNode(string childStructureName, string idReferencePath, string objectReferencePath)
        {
            ChildStructureName = childStructureName.AssertNotNullOrWhiteSpace("childStructureName");
            IdReferencePath = idReferencePath.AssertNotNullOrWhiteSpace("idReferencePath");
            ObjectReferencePath = objectReferencePath.AssertNotNullOrWhiteSpace("objectReferencePath");
        }
    }
}