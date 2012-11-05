using System;
using SisoDb.Annotations;

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