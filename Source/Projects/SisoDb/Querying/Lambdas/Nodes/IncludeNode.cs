using System;
using EnsureThat;
using PineCone.Structures;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class IncludeNode : INode
    {
        public string ReferencedStructureName { get; private set; }

        public string IdReferencePath { get; private set; }

        public string ObjectReferencePath { get; private set; }

        public Type DataType { get; private set; }

        public DataTypeCode DataTypeCode { get; private set; }

        public IncludeNode(string referencedStructureName, string idReferencePath, string objectReferencePath, Type dataType, DataTypeCode dataTypeCode)
        {
			Ensure.That(referencedStructureName, "referencedStructureName").IsNotNullOrWhiteSpace();
            Ensure.That(idReferencePath, "idReferencePath").IsNotNullOrWhiteSpace();
            Ensure.That(objectReferencePath, "objectReferencePath").IsNotNullOrWhiteSpace();
            Ensure.That(dataType, "dataType").IsNotNull();

            ReferencedStructureName = referencedStructureName;
            IdReferencePath = idReferencePath;
            ObjectReferencePath = objectReferencePath;
            DataType = dataType;
            DataTypeCode = dataTypeCode;
        }
    }
}