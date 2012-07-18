using System;
using SisoDb.PineCone.Annotations;

namespace SisoDb.Testing.TestModel
{
    [Serializable]
    public class UniqueStringItem
    {
        public string StructureId { get; set; }

        [Unique(UniqueModes.PerType)]
        public int UniqueValue { get; set; }
    }
}