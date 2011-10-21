using System;

namespace SisoDb.Testing.TestModel
{
    [Serializable]
    public class BigIdentityItem
    {
        public long StructureId { get; set; }

        public long Value { get; set; }
    }
}