using System;
using PineCone.Annotations;

namespace SisoDb.Testing.TestModel
{
    public class UniqueGuidItem
    {
        public Guid StructureId { get; set; }

        [Unique(UniqueModes.PerType)]
        public int Value { get; set; }
    }
}