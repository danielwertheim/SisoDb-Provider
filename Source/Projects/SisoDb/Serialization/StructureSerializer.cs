using SisoDb.Structures.Schemas;

namespace SisoDb.Serialization
{
    public class StructureSerializer : IStructureSerializer
    {
        private readonly ISisoSerializer _serializer;

        public StructureSerializer(ISisoSerializer serializer)
        {
            _serializer = serializer;
        }

        public string Serialize<T>(T item, IStructureSchema structureSchema) where T : class
        {
            return _serializer.Serialize(item);
        }
    }
}