using System;
using SisoDb.Annotations;

namespace SisoDb.Testing.TestModel
{
    [Serializable]
    public class UniqueGuidItem
    {
        public Guid StructureId { get; set; }

        [Unique(UniqueModes.PerType)]
        public int UniqueValue { get; set; }
    }
}