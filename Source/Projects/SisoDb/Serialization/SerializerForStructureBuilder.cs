using PineCone.Serializers;

namespace SisoDb.Serialization
{
    public class SerializerForStructureBuilder : ISerializer
    {
        public string Serialize<T>(T item) where T : class
        {
            return ServiceStackJsonSerializer<T>.Serialize(item);
        }
    }
}