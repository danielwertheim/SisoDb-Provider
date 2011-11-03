using System;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using PineCone.Structures.Schemas.Builders;
using SisoDb.Serialization;

namespace SisoDb
{
    public class ResourceContainer
    {
        private readonly IJsonSerializer _defaultJsonSerializer;
        
        public Func<IJsonSerializer> ResolveJsonSerializer;
        public Func<IStructureSchemas> ResolveStructureSchemas;
        public Func<IStructureBuilder> ResolveStructureBuilder;

        public ResourceContainer()
        {
            _defaultJsonSerializer = new ServiceStackJsonSerializer();

            ResolveJsonSerializer = () => _defaultJsonSerializer;
            ResolveStructureSchemas = () => new StructureSchemas(new StructureTypeFactory(), new AutoSchemaBuilder());
            ResolveStructureBuilder = () => new StructureBuilder();
        }
    }
}