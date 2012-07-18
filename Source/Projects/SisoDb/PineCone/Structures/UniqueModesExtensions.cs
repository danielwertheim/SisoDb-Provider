using System;
using PineCone.Annotations;
using PineCone.Resources;

namespace PineCone.Structures
{
    public static class UniqueModesExtensions
    {
        public static StructureIndexType ToStructureIndexType(this UniqueModes? uniqueMode)
        {
            if (uniqueMode == null)
                return StructureIndexType.Normal;

            return uniqueMode.Value.ToStructureIndexType();
        }

        public static StructureIndexType ToStructureIndexType(this UniqueModes uniqueMode)
        {
            if (uniqueMode == UniqueModes.PerInstance)
                return StructureIndexType.UniquePerInstance;

            if (uniqueMode == UniqueModes.PerType)
                return StructureIndexType.UniquePerType;

            throw new NotSupportedException(ExceptionMessages.UniqueModesCantBeMapped);
        }
    }
}