using System;
using SisoDb.Annotations;

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