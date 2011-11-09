using System;
using PineCone.Annotations;

namespace SisoDb.Testing.TestModel
{
    [Serializable]
    public class UniqueBigIdentityItem
    {
        public long StructureId { get; set; }

        [Unique(UniqueModes.PerType)]
        public long UniqueValue { get; set; }
    }
}