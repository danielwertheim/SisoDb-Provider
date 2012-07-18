using System;

namespace SisoDb.Testing.TestModel
{
    [Serializable]
    public class GuidItem
    {
        public Guid StructureId { get; set; }
        public int Value { get; set; }
        public Guid GuidValue { get; set; }
    }
}