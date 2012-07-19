using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SisoDb.EnsureThat;
using SisoDb.PineCone.Structures.Schemas.Builders;

namespace SisoDb.PineCone.Structures.Schemas
{
    public class StructureSchemas : IStructureSchemas
    {
        private readonly ConcurrentDictionary<Type, IStructureSchema> _schemas;

        private readonly Func<Type, IStructureSchema> _schemaFactoryFn;

        public IStructureTypeFactory StructureTypeFactory { get; set; }
        
        public ISchemaBuilder SchemaBuilder { get; set; }
        
        public StructureSchemas(IStructureTypeFactory structureTypeFactory, ISchemaBuilder schemaBuilder)
        {
            Ensure.That(structureTypeFactory, "structureTypeFactory").IsNotNull();
            Ensure.That(schemaBuilder, "schemaBuilder").IsNotNull();

            StructureTypeFactory = structureTypeFactory;
            SchemaBuilder = schemaBuilder;
            _schemas = new ConcurrentDictionary<Type, IStructureSchema>();

            _schemaFactoryFn = t => SchemaBuilder.CreateSchema(StructureTypeFactory.CreateFor(t));
        }

        public IStructureSchema GetSchema<T>() where T : class 
        {
            return GetSchema(typeof(T));
        }

        public IStructureSchema GetSchema(Type type)
        {
            Ensure.That(type, "type").IsNotNull();

            return _schemas.GetOrAdd(type, _schemaFactoryFn);
        }

        public IEnumerable<IStructureSchema> GetSchemas()
        {
            return _schemas.Values;
        }

        public void RemoveSchema(Type type)
        {
            Ensure.That(type, "type").IsNotNull();

            IStructureSchema tmp;

            _schemas.TryRemove(type, out tmp);
        }

        public IEnumerable<KeyValuePair<Type, IStructureSchema>> GetRegistrations()
        {
            return _schemas.ToList();
        }

        public void Clear()
        {
            _schemas.Clear();
        }
    }
}