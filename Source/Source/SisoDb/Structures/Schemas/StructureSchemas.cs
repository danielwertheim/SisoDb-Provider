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

        public IStructureSchema GetSchema(IStructureType structureType)
        {
            if (!_schemas.ContainsKey(structureType.Name))
                Register(structureType);

            return _schemas[structureType.Name];
        }

        public void RemoveSchema(IStructureType structureType)
        {
            _schemas.Remove(structureType.Name);
        }

        public void Clear()
        {
            _schemas.Clear();
        }

        private void Register(IStructureType structureType)
        {
            _schemas.Add(
                structureType.Name,
                new AutoSchemaBuilder(structureType).CreateSchema());
        }
    }
}