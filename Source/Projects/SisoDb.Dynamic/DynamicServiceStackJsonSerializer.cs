using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;
using SisoDb.Serialization;

namespace SisoDb.Dynamic
{
    public class DynamicServiceStackJsonSerializer : IDynamicJsonSerializer
    {
        public IDictionary<string, object> ToTypedKeyValueOrNull(TypeDescriptor typeDescriptor, string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            var kvRepresentation = ServiceStackJsonSerializer<IDictionary<string, dynamic>>.ToItemOrNull(json);
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