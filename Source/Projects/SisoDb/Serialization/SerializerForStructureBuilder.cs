using PineCone.Serializers;

namespace SisoDb.Serialization
{
    public class SerializerForStructureBuilder : IStructureSerializer
    {
        private static readonly IJsonSerializer Serializer;

        static SerializerForStructureBuilder()
        {
            Serializer = SisoEnvironment.Resources.ResolveJsonSerializer();
        }

        public string Serialize<T>(T item) where T : class
        {
            return Serializer.Serialize(item);
        }
    }
}