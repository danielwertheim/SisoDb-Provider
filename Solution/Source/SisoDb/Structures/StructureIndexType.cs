using System;

namespace SisoDb.Structures
{
    [Serializable]
    public enum StructureIndexType
    {
        Normal = 0,
        UniquePerType = 1,
        UniquePerInstance = 2
    }

    public static class StructureIndexTypeExtensions
    {
        public static bool IsUnique(this StructureIndexType indexType)
        {
            return indexType == StructureIndexType.UniquePerInstance || indexType == StructureIndexType.UniquePerType;
        }
    }
}