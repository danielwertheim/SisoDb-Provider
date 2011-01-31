using System.Collections.Generic;

namespace SisoDb.Structures.Schemas
{
    internal class StructureSchemas : IStructureSchemas
    {
        private readonly Dictionary<string, IStructureSchema> _schemas;

        public StructureSchemas()
        {
            _schemas = new Dictionary<string, IStructureSchema>();
        }

        public void Register<T>() where T : class
        {
            var builder = new AutoSchemaBuilder<T>();
            var schema = builder.CreateSchema();

            Add(schema);
        }

        private void Add(IStructureSchema structureSchema)
        {
            _schemas.Add(structureSchema.Name, structureSchema);
        }

        public IStructureSchema GetSchema<T>() where T : class
        {
            var key = TypeInfo<T>.Name;

            if (!_schemas.ContainsKey(key))
                Register<T>();

            return _schemas[TypeInfo<T>.Name];
        }
    }
}