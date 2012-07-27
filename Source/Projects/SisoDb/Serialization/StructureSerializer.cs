using SisoDb.PineCone.Serializers;
using SisoDb.PineCone.Structures.Schemas;

namespace SisoDb.Serialization
{
    public class StructureSerializer : IStructureSerializer
    {
        private readonly ISisoDbSerializer _serializer;

        public StructureSerializer(ISisoDbSerializer serializer)
        {
            _serializer = serializer;
        }

        public string Serialize<T>(T item, IStructureSchema structureSchema) where T : class
        {
            return _serializer.Serialize(item, structureSchema);
        }
    }
}