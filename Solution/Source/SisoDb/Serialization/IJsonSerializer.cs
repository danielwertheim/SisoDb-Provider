using System.Collections.Generic;
using SisoDb.Dynamic;

namespace SisoDb.Serialization
{
    public interface IJsonSerializer
    {
        string ToJsonOrEmptyString<T>(T item) where T : class;

        T ToItemOrNull<T>(string json) where T : class;

        IDictionary<string, object> ToTypedKeyValueOrNull(TypeDescriptor typeDescriptor, string json);
    }
}