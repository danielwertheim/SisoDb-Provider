using System;

namespace SisoDb.Testing.TestModel
{
    [Serializable]
    public class IdentityItemWithPrivateIdSetter
    {
        public IdentityItemWithPrivateIdSetter() { }

        public IdentityItemWithPrivateIdSetter(int id)
        {
            StructureId = id;
        }

        public int StructureId { get; private set; }

        public int Value { get; set; }
    }
}