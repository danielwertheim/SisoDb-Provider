using System.Collections.Generic;
using System.Linq;
using SisoDb.Serialization;

namespace SisoDb.Dynamic
{
    public class DynamicServiceStackJsonSerializer : IDynamicJsonSerializer
    {
        private static readonly IJsonSerializer Serializer = SisoEnvironment.Resources.ResolveJsonSerializer();

        public IDictionary<string, object> ToTypedKeyValueOrNull(TypeDescriptor typeDescriptor, string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            var kvRepresentation = Serializer.Deserialize<IDictionary<string, dynamic>>(json);
            if (kvRepresentation == null || kvRepresentation.Count < 1)
                return null;

            foreach (var key in kvRepresentation.Keys.ToArray())
            {
                var membername = key;
                var member = typeDescriptor.Get(membername);

                if (member == null)
                    continue;

                kvRepresentation[membername] = JsonSerializer.DeserializeFromString(kvRepresentation[membername], member.Type);
            }

            return kvRepresentation;
        }
    }
}