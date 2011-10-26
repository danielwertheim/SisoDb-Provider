using System.Collections.Generic;
using System.Linq;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.DbSchema;

namespace SisoDb.Dac.BulkInserts
{
    public class DbBulkInserter : IDbBulkInserter
    {
        private readonly IDbClient _dbClient;

        public DbBulkInserter(IDbClient dbClient)
        {
            _dbClient = dbClient;
        }

        public void Insert(IStructureSchema structureSchema, IEnumerable<IStructure> structures)
        {
            var structureStorageSchema = new StructureStorageSchema(structureSchema);
            var indexesStorageSchema = new IndexStorageSchema(structureSchema);
            var uniquesStorageSchema = new UniqueStorageSchema(structureSchema);
            
            using (var structuresReader = new StructuresReader(structureStorageSchema, structures))
            {
                using (var indexesReader = new IndexesReader(indexesStorageSchema, structures.SelectMany(s => s.Indexes)))
                {
                    InsertStructures(structuresReader);
                    InsertIndexes(indexesReader);

                    var uniques = structures.SelectMany(s => s.Uniques).ToArray();
                    if (uniques.Length <= 0)
                        return;
                    using (var uniquesReader = new UniquesReader(
                        uniquesStorageSchema,
                        uniques))
                    {
                        InsertUniques(uniquesReader);
                    }
                }
            }
        }

        private void InsertStructures(StructuresReader structures)
        {
            using (var bulkInserter = _dbClient.GetBulkCopy(true))
            {
                bulkInserter.BatchSize = structures.RecordsAffected;
                bulkInserter.DestinationTableName = structures.StorageSchema.Name;
                
                foreach (var field in structures.StorageSchema.GetFieldsOrderedByIndex())
                    bulkInserter.AddColumnMapping(field.Name, field.Name);

                bulkInserter.Write(structures);
            }
        }

        private void InsertIndexes(IndexesReader indexes)
        {
            using (var bulkInserter = _dbClient.GetBulkCopy(true))
            {
                bulkInserter.BatchSize = indexes.RecordsAffected;
                bulkInserter.DestinationTableName = indexes.StorageSchema.Name;

                foreach (var field in indexes.StorageSchema.GetFieldsOrderedByIndex())
                    bulkInserter.AddColumnMapping(field.Name, field.Name);

                bulkInserter.Write(indexes);
            }
        }

        private void InsertUniques(UniquesReader uniques)
        {
            using (var bulkInserter = _dbClient.GetBulkCopy(false))
            {
                bulkInserter.BatchSize = uniques.RecordsAffected;
                bulkInserter.DestinationTableName = uniques.StorageSchema.Name;

                foreach (var field in uniques.StorageSchema.GetFieldsOrderedByIndex())
                    bulkInserter.AddColumnMapping(field.Name, field.Name);

                bulkInserter.Write(uniques);
            }
        }
    }
}