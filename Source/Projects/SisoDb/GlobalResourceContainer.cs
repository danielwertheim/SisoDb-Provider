using System;
using SisoDb.Serialization;

namespace SisoDb
{
    public class GlobalResourceContainer
    {
        private readonly IJsonSerializer _defaultJsonSerializer;
        
        public Func<IJsonSerializer> ResolveJsonSerializer; //TODO: Rename JsonSerializerResolver

        public GlobalResourceContainer()
        {
            _defaultJsonSerializer = new ServiceStackJsonSerializer();

            ResolveJsonSerializer = () => _defaultJsonSerializer;
        }
    }
}