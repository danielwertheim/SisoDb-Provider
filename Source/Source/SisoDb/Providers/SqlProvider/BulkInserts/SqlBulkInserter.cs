using System.Collections.Generic;
using System.Linq;
using SisoDb.Providers.SqlProvider.DbSchema;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.BulkInserts
{
    internal class SqlBulkInserter
    {
        private readonly ISqlDbClient _dbClient;
        private readonly IElementBatcher _elementsBatcher;

        internal SqlBulkInserter(ISqlDbClient dbClient)
        {
            _dbClient = dbClient;
            _elementsBatcher = new ElementBatcher(1000);
        }

        internal void Insert(IStructureSchema structureSchema, IEnumerable<IStructure> structures)
        {
            var structureStorageSchema = new StructureStorageSchema(structureSchema);
            var indexesStorageSchema = new IndexStorageSchema(structureSchema);
            var uniquesStorageSchema = new UniqueStorageSchema(structureSchema);

            foreach (var batch in _elementsBatcher.Batch(structures))
            {
                using (var structuresReader = new StructuresReader(structureStorageSchema, batch))
                {
                    var indexRows = batch.Select(s => new IndexRow(s.Id, s.Indexes.ToArray()));
                    using (var indexesReader = new IndexesReader(indexesStorageSchema, indexRows))
                    {
                        using (var uniquesReader = new UniquesReader(uniquesStorageSchema, batch.SelectMany(s => s.Uniques).ToList()))
                        {
                            InsertStructures(structuresReader);
                            InsertIndexes(indexesReader);
                            InsertUniques(uniquesReader);
                        }
                    }
                }
            }
        }

        private void InsertStructures(StructuresReader structures)
        {
            using (var bulkInserter = _dbClient.GetBulkCopy())
            {
                bulkInserter.BatchSize = structures.RecordsAffected;
                bulkInserter.DestinationTableName = structures.StorageSchema.Name;
                bulkInserter.NotifyAfter = 0;

                foreach (var field in structures.StorageSchema.FieldsByIndex.Values)
                    bulkInserter.ColumnMappings.Add(field.Name, field.Name);

                bulkInserter.WriteToServer(structures);
                bulkInserter.Close();
            }
        }

        private void InsertIndexes(IndexesReader indexes)
        {
            using (var bulkInserter = _dbClient.GetBulkCopy())
            {
                bulkInserter.BatchSize = indexes.RecordsAffected;
                bulkInserter.DestinationTableName = indexes.StorageSchema.Name;
                bulkInserter.NotifyAfter = 0;

                foreach (var field in indexes.StorageSchema.FieldsByIndex.Values)
                    bulkInserter.ColumnMappings.Add(field.Name, field.Name);

                bulkInserter.WriteToServer(indexes);
                bulkInserter.Close();
            }
        }

        private void InsertUniques(UniquesReader uniques)
        {
            using (var bulkInserter = _dbClient.GetBulkCopy())
            {
                bulkInserter.BatchSize = uniques.RecordsAffected;
                bulkInserter.DestinationTableName = uniques.StorageSchema.Name;
                bulkInserter.NotifyAfter = 0;

                foreach (var field in uniques.StorageSchema.FieldsByIndex.Values)
                    bulkInserter.ColumnMappings.Add(field.Name, field.Name);

                bulkInserter.WriteToServer(uniques);
                bulkInserter.Close();
            }
        }
    }
}