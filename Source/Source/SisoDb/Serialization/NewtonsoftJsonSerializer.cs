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

            return JsonConvert.SerializeObjectAs<T, T>(item, Formatting.None, SerializerSettings);
        }

        public T ToItemOrNull<T>(string json) where T : class
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            return JsonConvert.DeserializeObject<T>(json, DeserializerSettings);
        }
    }
}