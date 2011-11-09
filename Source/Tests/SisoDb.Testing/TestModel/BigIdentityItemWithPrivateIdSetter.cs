using System;

namespace SisoDb.Testing.TestModel
{
    [Serializable]
    public class BigIdentityItemWithPrivateIdSetter
    {
        public BigIdentityItemWithPrivateIdSetter() { }

        public BigIdentityItemWithPrivateIdSetter(long id)
        {
            StructureId = id;
        }

        public long StructureId { get; private set; }

        public long Value { get; set; }
    }
}