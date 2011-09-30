using System;
using SisoDb.Annotations;
using SisoDb.Resources;

namespace SisoDb.Structures
{
    internal static class UniqueModesExtensions
    {
        internal static StructureIndexType ToStructureIndexType(this UniqueModes uniqueModes)
        {
            if (uniqueModes == UniqueModes.PerInstance)
                return StructureIndexType.UniquePerInstance;

            if (uniqueModes == UniqueModes.PerType)
                return StructureIndexType.UniquePerType;

            throw new NotSupportedException(ExceptionMessages.UniqueModesCantBeMapped);
        }
    }
}