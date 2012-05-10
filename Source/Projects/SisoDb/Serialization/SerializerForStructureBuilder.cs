using PineCone.Serializers;

namespace SisoDb.Serialization
{
    public class SerializerForStructureBuilder : IStructureSerializer
    {
        private readonly IJsonSerializer _serializer;

        public SerializerForStructureBuilder(IJsonSerializer serializer)
        {
            _serializer = serializer;
        }

        public string Serialize<T>(T item) where T : class
        {
            return _serializer.Serialize(item);
        }
    }
}