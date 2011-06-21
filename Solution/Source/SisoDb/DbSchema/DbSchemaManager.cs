using System.Collections.Generic;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSchema
{
    public class DbSchemaManager : IDbSchemaManager
    {
        private readonly ISet<string> _upsertedSchemas;

        public DbSchemaManager()
        {
            _upsertedSchemas = new HashSet<string>();
        }

        public void DropStructureSet(IStructureSchema structureSchema, IDbSchemaDropper dropper)
        {
            lock (_upsertedSchemas)
            {
                _upsertedSchemas.Remove(structureSchema.Name);

                dropper.Drop(structureSchema);
            }
        }

        public void UpsertStructureSet(IStructureSchema structureSchema, IDbSchemaUpserter upserter)
        {
            lock (_upsertedSchemas)
            {
                if (_upsertedSchemas.Contains(structureSchema.Name))
                    return;

                upserter.Upsert(structureSchema);

                _upsertedSchemas.Add(structureSchema.Name);
            }
        }
    }
}