using System;

namespace SisoDb.Testing.TestModel
{
    [Serializable]
    public class GuidItemWithNullableId
    {
        public Guid? StructureId { get; set; }

        public int Value { get; set; }
    }
}