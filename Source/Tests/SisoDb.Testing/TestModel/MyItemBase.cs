using System;
using SisoDb.PineCone.Annotations;

namespace SisoDb.Testing.TestModel
{
    public abstract class MyItemBase
    {
        public Guid StructureId { get; set; }

        public int MyItemBaseInt { get; set; }

        [Unique(UniqueModes.PerType)]
        public int MyItemBaseUniqueInt { get; set; }
    }
}