using System;

namespace SisoDb.Testing.TestModel
{
    [Serializable]
    public class IdentityItemWithNullableId
    {
        public int? StructureId { get; set; }

        public int Value { get; set; }
    }
}