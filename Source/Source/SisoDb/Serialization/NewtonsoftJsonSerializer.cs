using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace SisoDb.Serialization
{
    /// <summary>
    /// <![CDATA[http://json.codeplex.com, http://james.newtonking.com/projects/json/help/]]>
    /// </summary>
    public class NewtonsoftJsonSerializer : IJsonSerializer
    {
        private static readonly JsonSerializerSettings SerializerSettings;

        private static readonly JsonSerializerSettings DeserializerSettings;

        //private static readonly IsoDateTimeConverter DateTimeConverter;

        static NewtonsoftJsonSerializer()
        {
            //DateTimeConverter = new IsoDateTimeConverter
            //{
            //    Culture = (CultureInfo)SisoDbEnvironment.DateTimeFormatProvider,
            //    DateTimeFormat = SisoDbEnvironment.DateTimePattern,
            //    DateTimeStyles = DateTimeStyles.None
            //};

            SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new SisoJsonDefaultContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
            //SerializerSettings.Converters.Add(DateTimeConverter);

            DeserializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new SisoJsonDefaultContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            //DeserializerSettings.Converters.Add(DateTimeConverter);
        }

        public string ToJsonOrEmptyString<T>(T item) where T : class
        {
            if (item == null)
                return string.Empty;

            var jsonSerializer = JsonSerializer.Create(SerializerSettings);
            var json = new StringBuilder(128);
            using (var writer = new StringWriter(json, CultureInfo.InvariantCulture))
            {
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    jsonWriter.Formatting = Formatting.None;
                    jsonSerializer.SerializeAs<T, T>(jsonWriter, item);
                }
            }
            return json.ToString();
        }

        public T ToItemOrNull<T>(string json) where T : class
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            var sr = new StringReader(json);
            var jsonSerializer = JsonSerializer.Create(DeserializerSettings);

            using (var jsonReader = new JsonTextReader(sr))
            {
                return jsonSerializer.Deserialize(jsonReader, typeof(T)) as T;
            }
        }
    }
}