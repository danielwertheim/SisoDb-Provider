using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SisoDb.Serialization
{
    /// <summary>
    /// <![CDATA[http://james.newtonking.com/projects/json/help/]]>
    /// </summary>
    internal static class JsonSerialization
    {

        private static readonly JsonSerializerSettings SerializerSettings;

        private static readonly JsonSerializerSettings DeserializerSettings;

        private static readonly IsoDateTimeConverter DateTimeConverter;

        static JsonSerialization()
        {
            DateTimeConverter = new IsoDateTimeConverter
            {
                Culture = (CultureInfo)SisoDbEnvironment.DateTimeFormatProvider,
                DateTimeFormat = SisoDbEnvironment.DateTimePattern,
                DateTimeStyles = DateTimeStyles.None
            };

            SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new SisoJsonDefaultContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
            SerializerSettings.Converters.Add(DateTimeConverter);

            DeserializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new SisoJsonDefaultContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            DeserializerSettings.Converters.Add(DateTimeConverter);
        }

        internal static string ToJsonOrEmptyString<T>(T item) where T : class
        {
            if (item == null)
                return string.Empty;

            var json = JsonConvert.SerializeObject(item, Formatting.None, SerializerSettings);

            return json;
        }

        internal static T ToItemOrNull<T>(string json) where T : class
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            var item = JsonConvert.DeserializeObject<T>(json, DeserializerSettings);

            return item;
        }
    }
}