using System;
using System.Collections.Generic;
using SisoDb.Core;

namespace SisoDb.Structures.Schemas
{
    public class StructureSchemas : IStructureSchemas
    {
        private readonly Dictionary<string, IStructureSchema> _schemas;

        public IStructureTypeFactory StructureTypeFactory { protected get; set; }
        public ISchemaBuilder SchemaBuilder { protected get; set; }
        
        public StructureSchemas(IStructureTypeFactory structureTypeFactory, ISchemaBuilder schemaBuilder)
        {
            StructureTypeFactory = structureTypeFactory.AssertNotNull("structureTypeFactory");
            SchemaBuilder = schemaBuilder.AssertNotNull("schemaBuilder");

            _schemas = new Dictionary<string, IStructureSchema>();
        }

        public IStructureSchema GetSchema(Type type)
        {
            type.AssertNotNull("type");

            if (!_schemas.ContainsKey(type.Name))
                Register(type);

            return _schemas[type.Name];
        }

        public void RemoveSchema(Type type)
        {
            type.AssertNotNull("type");

            _schemas.Remove(type.Name);
        }

        public void Clear()
        {
            _schemas.Clear();
        }

        private void Register(Type type)
        {
            var structureType = StructureTypeFactory.CreateFor(type);

            _schemas.Add(
                structureType.Name,
                SchemaBuilder.CreateSchema(structureType));
        }
    }
}