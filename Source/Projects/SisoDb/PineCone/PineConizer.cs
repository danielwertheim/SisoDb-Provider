using System.Collections.Generic;
using SisoDb.EnsureThat;
using SisoDb.PineCone.Structures;
using SisoDb.PineCone.Structures.Schemas;
using SisoDb.PineCone.Structures.Schemas.Builders;

namespace SisoDb.PineCone
{
    public class PineConizer : IPineConizer
    {
        private IStructureSchemas _schemas;
        private IStructureBuilder _builder;

        public IStructureSchemas Schemas
        {
            get { return _schemas; }
            set
            {
                Ensure.That(value, "Schemas").IsNotNull();

                _schemas = value;
            }
        }
        
        public IStructureBuilder Builder
        {
            get { return _builder; }
            set
            {
                Ensure.That(value, "Builder").IsNotNull();

                _builder = value;
            }
        }

        public PineConizer()
        {
            var structureTypeFactory = new StructureTypeFactory();
            var schemaBuilder = new AutoSchemaBuilder();

            Schemas = new StructureSchemas(structureTypeFactory, schemaBuilder);
            Builder = new StructureBuilder();
        }

        public IStructure CreateStructureFor<T>(T item) where T : class
        {
            return Builder.CreateStructure(item, Schemas.GetSchema<T>());
        }

        public IEnumerable<IStructure> CreateStructuresFor<T>(T[] items) where T : class
        {
            return Builder.CreateStructures(items, Schemas.GetSchema<T>());
        }
    }
}