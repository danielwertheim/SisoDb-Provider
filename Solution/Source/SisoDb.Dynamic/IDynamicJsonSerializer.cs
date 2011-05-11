using System.Collections.Generic;

namespace SisoDb.Dynamic
{
    public interface IDynamicJsonSerializer
    {
        IDictionary<string, object> ToTypedKeyValueOrNull(TypeDescriptor typeDescriptor, string json);
    }
}