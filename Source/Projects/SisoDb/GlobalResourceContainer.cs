using System;
using SisoDb.Serialization;

namespace SisoDb
{
    public class GlobalResourceContainer
    {
        private readonly IJsonSerializer _defaultJsonSerializer;
        
        public Func<IJsonSerializer> ResolveJsonSerializer;

        public GlobalResourceContainer()
        {
            _defaultJsonSerializer = new ServiceStackJsonSerializer();

            ResolveJsonSerializer = () => _defaultJsonSerializer;
        }
    }
}