using System;
using Newtonsoft.Json;

namespace SisoDb.JsonNet
{
    public class NullableGuidConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
                writer.WriteValue(((Guid)value).ToString("N"));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null || reader.Value == null)
                return null;

            var value = reader.Value.ToString();
            return string.IsNullOrWhiteSpace(value)
                       ? Guid.Empty
                       : Guid.Parse(value);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (Guid?);
        }
    }
}