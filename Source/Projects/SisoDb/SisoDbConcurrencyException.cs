using System;

namespace SisoDb
{
    [Serializable]
    public class SisoDbConcurrencyException : SisoDbException
    {
        public object StructureId { get; private set; }
        public string StructureName { get; private set; }

        public SisoDbConcurrencyException(object structureId, string structureName, string message)
            : base(message)
        {
            StructureId = structureId;
            StructureName = structureName;
        }
    }
}