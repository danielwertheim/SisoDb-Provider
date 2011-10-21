using System;

namespace SisoDb.Testing.TestModel
{
    [Serializable]
    public class BigIdentityItemWithNullableId
    {
        public long? StructureId { get; set; }

        public long Value { get; set; }
    }
}