using System;

namespace SisoDb.Testing.TestModel
{
    [Serializable]
    public class GuidItemWithPrivateIdSetter
    {
        public GuidItemWithPrivateIdSetter() { }

        public GuidItemWithPrivateIdSetter(Guid id)
        {
            StructureId = id;
        }

        public Guid StructureId { get; private set; }

        public int Value { get; set; }
    }
}