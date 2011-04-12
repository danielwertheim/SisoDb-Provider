using System;
using SisoDb.Annotations;
using SisoDb.Resources;

namespace SisoDb.Structures
{
    internal static class UniqueModesExtensions
    {
        internal static StructureIndexUniques ToStructureIndexUniques(this UniqueModes uniqueModes)
        {
            if (uniqueModes == UniqueModes.PerInstance)
                return StructureIndexUniques.PerInstance;

            if (uniqueModes == UniqueModes.PerType)
                return StructureIndexUniques.PerType;

            throw new NotSupportedException(ExceptionMessages.UniqueModesCantBeMapped);
        }
    }
}