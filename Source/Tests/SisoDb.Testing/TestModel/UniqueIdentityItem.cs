using PineCone.Annotations;

namespace SisoDb.Testing.TestModel
{
    public class UniqueIdentityItem
    {
        public int StructureId { get; set; }

        [Unique(UniqueModes.PerType)]
        public int Value { get; set; }
    }
}