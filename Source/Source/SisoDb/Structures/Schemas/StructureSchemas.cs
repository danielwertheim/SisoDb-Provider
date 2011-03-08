using System;
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
            var key = StructureType<T>.Name;

            if (!_schemas.ContainsKey(key))
                Register<T>(key);

            return _schemas[key];
        }

        public void RemoveSchema<T>() where T : class
        {
            var key = StructureType<T>.Name;
            _schemas.Remove(key);
        }

        public void Clear()
        {
            _schemas.Clear();
        }

        private void Register<T>(string key) where T : class
        {
            var schema = new AutoSchemaBuilder<T>().CreateSchema();

            _schemas.Add(key, schema);
        }
    }
}