using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using SisoDb.EnsureThat;
using SisoDb.Serialization;

namespace SisoDb.JsonNet
{
    public class JsonNetSisoSerializer : ISisoSerializer
    {
        protected readonly JsonSerializerSettings Settings;

        public JsonNetSisoSerializer(Action<JsonSerializerSettings> cfg = null)
        {
            Settings = CreateDefaultSettings();
            if (cfg != null)
                cfg(Settings);
        }

        protected static JsonSerializerSettings CreateDefaultSettings()
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                CheckAdditionalContent = false,
                ConstructorHandling = ConstructorHandling.Default,
                TypeNameHandling = TypeNameHandling.None,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = DateParseHandling.DateTime,
                DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver()
            };
            settings.Converters.Add(new StringEnumConverter());
            settings.Converters.Add(new GuidConverter());
            settings.Converters.Add(new NullableGuidConverter());

            return settings;
        }

        public virtual string Serialize<T>(T item) where T : class
        {
            if (item == null)
                return string.Empty;

            return JsonConvert.SerializeObject(item, Settings);
        }

        public virtual IEnumerable<string> SerializeMany<T>(IEnumerable<T> items) where T : class
        {
            foreach (var item in items)
            {
                if (item == null)
                {
                    yield return string.Empty;
                    continue;
                }

                yield return JsonConvert.SerializeObject(item);
            }
        }

        public virtual T Deserialize<T>(string json) where T : class
        {
            return OnDeserialize<T>(json);
        }

        public virtual object Deserialize(string json, Type structureType)
        {
            Ensure.That(structureType, "structureType").IsNotNull();

            return OnDeserialize(json, structureType);
        }

        public virtual IEnumerable<T> DeserializeMany<T>(IEnumerable<string> sourceData) where T : class
        {
            return sourceData.Select(OnDeserialize<T>);
        }

        public virtual IEnumerable<object> DeserializeMany(IEnumerable<string> sourceData, Type structureType)
        {
            Ensure.That(structureType, "structureType").IsNotNull();

            return sourceData.Select(json => OnDeserialize(json, structureType));
        }

        protected virtual T OnDeserialize<T>(string json) where T : class
        {
            return string.IsNullOrEmpty(json)
                ? null
                : JsonConvert.DeserializeObject<T>(json, Settings);
        }

        protected virtual object OnDeserialize(string json, Type structureType)
        {
            return string.IsNullOrEmpty(json)
                ? null
                : JsonConvert.DeserializeObject(json, structureType, Settings);
        }
    }
}