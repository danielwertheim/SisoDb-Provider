using System.Collections.Generic;
using System.Data;
using System.Linq;
using SisoDb.Providers.AzureProvider.DbSchema;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    /// <summary>
    /// Adds missing columns and Drops obsolete columns; to Indexes-table.
    /// </summary>
    /// <remarks>The table must exist, otherwise an Exception is thrown!</remarks>
    internal class SqlDbIndexesSchemaSynchronizer : ISqlDbSchemaSynchronizer
    {
        private readonly SqlDbClient _dbClient;
        private readonly ISqlStrings _sqlStrings;
        private readonly ISqlDbColumnGenerator _columnGenerator;
        private readonly SqlDbDataTypeTranslator _dataTypeTranslator;

        internal SqlDbIndexesSchemaSynchronizer(SqlDbClient dbClient)
        {
            _dbClient = dbClient;
            _sqlStrings = dbClient.SqlStrings;
            _dataTypeTranslator = new SqlDbDataTypeTranslator();

            //TODO: IoC or Factory
            _columnGenerator = dbClient.ProviderType == StorageProviders.Sql2008 ? 
                (ISqlDbColumnGenerator)new SqlDbColumnGenerator() : new AzureDbColumnGenerator();
        }

        public void Synchronize(IStructureSchema structureSchema)
        {
            var tableName = structureSchema.GetIndexesTableName();

            var schemaChanges = GetSchemaChanges(structureSchema);
            if (schemaChanges.Count() < 1)
                return;

            var columnsToDrop = schemaChanges
                .Where(sc => sc.Change == SchemaChanges.IsObsoleteColumn)
                .ToList();
            DropColumns(tableName, columnsToDrop);

            var columnsToAdd = schemaChanges
                .Where(sc => sc.Change == SchemaChanges.IsMissingColumn)
                .ToList();
            AddColumns(tableName, columnsToAdd);
        }

        private void AddColumns(string tableName, IList<SchemaChange> columns)
        {
            if (columns == null || columns.Count < 1)
                return;

            var columnsDdl = columns.Select(sc => _columnGenerator.ToSql(sc.Name, sc.DbDataType));
            var columnsDdlCombined = string.Join(",", columnsDdl);
            var sql = _sqlStrings.GetSql("AddColumns")
                .Inject(tableName, columnsDdlCombined);

            _dbClient.ExecuteNonQuery(CommandType.Text, sql);
        }

        private void DropColumns(string tableName, IList<SchemaChange> columns)
        {
            if (columns == null || columns.Count < 1)
                return;

            var columnNames = columns.Select(sc => "[" + sc.Name + "]");
            var namesCombined = string.Join(",", columnNames);
            var sql = _sqlStrings.GetSql("DropColumns")
                .Inject(tableName, namesCombined);

            _dbClient.ExecuteNonQuery(CommandType.Text, sql);
        }

        private IEnumerable<SchemaChange> GetSchemaChanges(IStructureSchema structureSchema)
        {
            var changes = new List<SchemaChange>();
            var indexAccessors = new List<IIndexAccessor>(structureSchema.IndexAccessors);
            var dbColumns = GetIndexesColumns(structureSchema);

            for (var c = dbColumns.Count - 1; c > -1; c--)
            {
                var dbColumn = dbColumns[c];
                var existsInBothSchemas = indexAccessors.Any(iac => iac.Name == dbColumn.Name);

                if (!existsInBothSchemas)
                    changes.Add(new SchemaChange(SchemaChanges.IsObsoleteColumn, dbColumn.Name, dbColumn.DbDataType));

                dbColumns.RemoveAt(c);
                indexAccessors.RemoveAll(iac => iac.Name == dbColumn.Name);
            }

            var missingColumnsInDb = indexAccessors.Select(
                iac => new SchemaChange(SchemaChanges.IsMissingColumn, iac.Name, _dataTypeTranslator.ToDbType(iac)));
            changes.AddRange(missingColumnsInDb);

            return changes;
        }

        private IList<SqlDbColumn> GetIndexesColumns(IStructureSchema structureSchema)
        {
            var dbColumns = _dbClient.GetColumns(structureSchema.GetIndexesTableName(), IndexStorageSchema.Fields.StructureId.Name);

            return dbColumns ?? new List<SqlDbColumn>();
        }
    }
}