using System;
using SisoDb.PineCone.Annotations;

namespace SisoDb.Testing.TestModel
{
    [Serializable]
    public class UniqueIdentityItem
    {
        public int StructureId { get; set; }

        [Unique(UniqueModes.PerType)]
        public int UniqueValue { get; set; }
    }
}