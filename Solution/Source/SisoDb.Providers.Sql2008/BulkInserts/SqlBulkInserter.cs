using System.Collections.Generic;
using System.Linq;
using SisoDb.Providers.BulkInserts;
using SisoDb.Providers.DbSchema;
using SisoDb.Providers.Sql2008.Dac;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.Sql2008.BulkInserts
{
    public class SqlBulkInserter
    {
        private readonly SqlDbClient _dbClient;

        public SqlBulkInserter(SqlDbClient dbClient)
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
                using (var indexesReader = new IndexesReader(indexesStorageSchema, ExtractIndexes(structures)))
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

        private static IEnumerable<IStructureIndex[]> ExtractIndexes(IEnumerable<IStructure> structures)
        {
            return structures.Select(s => s.Indexes.ToArray());
        }

        private void InsertStructures(StructuresReader structures)
        {
            using (var bulkInserter = _dbClient.GetBulkCopy(true))
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
            using (var bulkInserter = _dbClient.GetBulkCopy(true))
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
            using (var bulkInserter = _dbClient.GetBulkCopy(false))
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