using System;
using System.Data;
using System.Linq;
using SisoDb.Core;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Sql2008.Dac
{
    public class SqlSingleInserter
    {
        private readonly SqlDbClient _dbClient;

        public SqlSingleInserter(SqlDbClient dbClient)
        {
            _dbClient = dbClient;
        }

        public void Insert(IStructureSchema structureSchema, IStructure structure)
        {
            InsertIntoStructureTable(structureSchema, structure);

            InsertIntoIndexesTable(structureSchema, structure);

            InsertIntoUniquesTable(structureSchema, structure);
        }

        private void InsertIntoStructureTable(IStructureSchema structureSchema, IStructure structure)
        {
            var sql = "insert into [dbo].[{0}] ([{1}], [{2}]) values (@{1}, @{2});".Inject(
                structureSchema.GetStructureTableName(),
                StructureStorageSchema.Fields.Id.Name,
                StructureStorageSchema.Fields.Json.Name);

            var enableIdentityInserts = structureSchema.IdAccessor.IdType == IdTypes.Identity;
            if (enableIdentityInserts)
                sql = "set identity_insert [dbo].[{0}] on; {1}; set identity_insert [dbo].[{0}] off;".Inject(
                    structureSchema.GetStructureTableName(),
                    sql);

            using (var dbCommand = _dbClient.CreateCommand(
                CommandType.Text, sql,
                new DacParameter(StructureStorageSchema.Fields.Id.Name, structure.Id.Value),
                new DacParameter(StructureStorageSchema.Fields.Json.Name, structure.Json)))
            {
                dbCommand.ExecuteNonQuery();
            }
        }

        private void InsertIntoIndexesTable(IStructureSchema structureSchema, IStructure structure)
        {
            var storageSchema = new IndexStorageSchema(structureSchema);
            var cmdParamNames = string.Join(",", storageSchema.GetFieldsOrderedByIndex().Select(f => "@{0}".Inject(f.Name.Replace(".", "_"))));
            var sql = "insert into [dbo].[{0}] ({1}) values ({2});".Inject(
                structureSchema.GetIndexesTableName(),
                storageSchema.GetFieldsAsDelimitedOrderedString(),
                cmdParamNames);

            var indexesDictionary = structure.Indexes.ToDictionary(i => i.Name);

            var dbCommandParams = storageSchema.GetFieldsOrderedByIndex().Select(
                f =>
                {
                    if (f.Name == IndexStorageSchema.Fields.SisoId.Name)
                        return new DacParameter(f.Name, structure.Id.Value);

                    var value = indexesDictionary[f.Name].Value;

                    return value == null ? 
                        new DacParameter(f.Name.Replace(".", "_"), DBNull.Value) :
                        new DacParameter(f.Name.Replace(".", "_"), value);
                }).ToArray();

            using (var dbCommand = _dbClient.CreateCommand(CommandType.Text, sql, dbCommandParams))
            {
                dbCommand.ExecuteNonQuery();
            }
        }

        private void InsertIntoUniquesTable(IStructureSchema structureSchema, IStructure structure)
        {
            if (structure.Uniques.Count < 1)
                return;

            var storageSchema = new UniqueStorageSchema(structureSchema);
            var cmdParamNames = string.Join(",", storageSchema.GetFieldsOrderedByIndex().Select(f => "@" + f.Name));
            var sqlInsertIntoUniques = "insert into [dbo].[{0}] ({1}) values ({2});".Inject(
                structureSchema.GetUniquesTableName(),
                storageSchema.GetFieldsAsDelimitedOrderedString(),
                cmdParamNames);

            using (var dbCommand = _dbClient.CreateCommand(CommandType.Text, sqlInsertIntoUniques))
            {
                foreach (var unique in structure.Uniques)
                {
                    dbCommand.Parameters.Clear();

                    foreach (var schemaField in storageSchema.GetFieldsOrderedByIndex())
                    {
                        dbCommand.AddParameter(
                            schemaField.Name, 
                            GetUniquesDbParamValue(structure.Id, unique, schemaField));
                    }

                    dbCommand.ExecuteNonQuery();
                }
            }
        }

        private static object GetUniquesDbParamValue(ISisoId structureId, IStructureIndex uniqueIndex, SchemaField schemaField)
        {
            if (schemaField.Name == UniqueStorageSchema.Fields.SisoId.Name)
                return structureId.Value;

            if (schemaField.Name == UniqueStorageSchema.Fields.UqSisoId.Name)
            {
                if (uniqueIndex.IndexType == StructureIndexType.UniquePerInstance)
                    return structureId.Value;
                
                return DBNull.Value;
            }

            if (schemaField.Name == UniqueStorageSchema.Fields.UqName.Name)
                return uniqueIndex.Name;
            
            if (schemaField.Name == UniqueStorageSchema.Fields.UqValue.Name)
                return SisoEnvironment.Formatting.StringConverter.AsString(uniqueIndex.Value);
            
            throw new NotSupportedException();
        }
    }
}