using System;
using Newtonsoft.Json;

namespace SisoDb.JsonNet
{
    public class GuidConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteValue(Guid.Empty.ToString("N"));
            else
                writer.WriteValue(((Guid)value).ToString("N"));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if(reader.TokenType == JsonToken.Null || reader.Value == null)
                return Guid.Empty;

            var value = reader.Value.ToString();
            return string.IsNullOrWhiteSpace(value) 
                       ? Guid.Empty 
                       : Guid.Parse(value);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Guid);
        }
    }
}