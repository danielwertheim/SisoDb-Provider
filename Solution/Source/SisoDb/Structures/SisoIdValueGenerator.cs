using System;

namespace SisoDb.Structures
{
    internal static class SisoIdValueGenerator
    {
        internal static ValueType[] CreateGuidIds(int numOfIds)
        {
            var ids = new ValueType[numOfIds];

            for (var c = 0; c < numOfIds; c++)
                ids[c] = SequentialGuid.NewSqlCompatibleGuid();

            return ids;
        }

        internal static ValueType[] CreateIdentityIds(int seed, int numOfIds)
        {
            var ids = new ValueType[numOfIds];

            for (var c = 0; c < numOfIds; c++)
                ids[c] = seed + c;

            return ids;
        }
    }
}