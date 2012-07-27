using SisoDb.PineCone.Structures.Schemas;

namespace SisoDb.PineCone.Serializers
{
    public class EmptyStructureSerializer : IStructureSerializer
    {
        public string Serialize<T>(T item, IStructureSchema structureSchema) where T : class
        {
            return null;
        }
    }
}