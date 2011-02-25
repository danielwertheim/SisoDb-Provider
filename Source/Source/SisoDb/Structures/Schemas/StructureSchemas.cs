using System.Collections.Generic;

namespace SisoDb.Structures.Schemas
{
    public class StructureSchemas : IStructureSchemas
    {
        private readonly Dictionary<string, IStructureSchema> _schemas;

        public StructureSchemas()
        {
            _schemas = new Dictionary<string, IStructureSchema>();
        }

        public IStructureSchema GetSchema<T>() where T : class
        {
            var key = TypeInfo<T>.Name;

            if (!_schemas.ContainsKey(key))
                Register<T>();

            return _schemas[TypeInfo<T>.Name];
        }

        private void Register<T>() where T : class
        {
            var schema = new AutoSchemaBuilder<T>().CreateSchema();

            _schemas.Add(schema.Name, schema);
        }
    }
}