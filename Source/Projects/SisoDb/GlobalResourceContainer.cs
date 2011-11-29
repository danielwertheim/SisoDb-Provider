using System;
using PineCone.Structures.Schemas;
using PineCone.Structures.Schemas.Builders;
using SisoDb.Serialization;
using SisoDb.Structures;

namespace SisoDb
{
    public class GlobalResourceContainer
    {
        private readonly IJsonSerializer _defaultJsonSerializer;
        
        public Func<IJsonSerializer> ResolveJsonSerializer;
        public Func<IStructureSchemas> ResolveStructureSchemas;
        public Func<IStructureBuilders> ResolveStructureBuilders;

        public GlobalResourceContainer()
        {
            _defaultJsonSerializer = new ServiceStackJsonSerializer();

            ResolveJsonSerializer = () => _defaultJsonSerializer;
            ResolveStructureSchemas = () => new StructureSchemas(new StructureTypeFactory(), new AutoSchemaBuilder());
            ResolveStructureBuilders = () => new StructureBuilders();
        }
    }
}