using System.Collections.Generic;
using System.Data;
using System.Linq;
using NCore;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Providers;

namespace SisoDb.Sql2008.DbSchema
{
    public class SqlDbIndexesSchemaSynchronizer : IDbSchemaSynchronizer
    {
        private readonly IDbClient _dbClient;
        private readonly ISqlStatements _sqlStatements;

        public SqlDbIndexesSchemaSynchronizer(IDbClient dbClient)
        {
            _dbClient = dbClient;
            _sqlStatements = dbClient.SqlStatements;
        }

        public void Synchronize(IStructureSchema structureSchema)
        {
            var keyNamesToDrop = GetKeyNamesToDrop(structureSchema);

            if (keyNamesToDrop.Count > 0)
                DeleteRecordsMatchingKeyNames(structureSchema, keyNamesToDrop);
        }

        private void DeleteRecordsMatchingKeyNames(IStructureSchema structureSchema, IEnumerable<string> names)
        {
            var inString = string.Join(",", names.Select(n => "'" + n + "'"));
            var sql = _sqlStatements.GetSql("UniquesSchemaSynchronizer_DeleteRecordsMatchingKeyNames")
                .Inject(structureSchema.GetUniquesTableName(), UniqueStorageSchema.Fields.UqName.Name, inString);

            using (var cmd = _dbClient.CreateCommand(CommandType.Text, sql))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private IList<string> GetKeyNamesToDrop(IStructureSchema structureSchema)
        {
            var structureFields = new HashSet<string>(structureSchema.IndexAccessors.Select(iac => iac.Path));
            var keyNames = GetKeyNames(structureSchema);

            return keyNames.Where(kn => !structureFields.Contains(kn)).ToList();
        }

        private IEnumerable<string> GetKeyNames(IStructureSchema structureSchema)
        {
            var dbColumns = new List<string>();

            _dbClient.SingleResultSequentialReader(
                CommandType.Text,
                _sqlStatements.GetSql("UniquesSchemaSynchronizer_GetKeyNames").Inject(
                    UniqueStorageSchema.Fields.UqName.Name,
                    structureSchema.GetUniquesTableName()),
                    dr => dbColumns.Add(dr.GetString(0)));

            return dbColumns;
        }
    }

    ///// <summary>
    ///// Adds missing columns and Drops obsolete columns; to Indexes-table.
    ///// </summary>
    ///// <remarks>The table must exist, otherwise an Exception is thrown!</remarks>
    //public class SqlDbIndexesSchemaSynchronizer : IDbSchemaSynchronizer
    //{
    //    private readonly IDbClient _dbClient;
    //    private readonly IDbColumnGenerator _columnGenerator;
    //    private readonly SqlDbDataTypeTranslator _dataTypeTranslator;

    //    public SqlDbIndexesSchemaSynchronizer(IDbClient dbClient, IDbColumnGenerator columnGenerator)
    //    {
    //        Ensure.That(() => dbClient).IsNotNull();
    //        Ensure.That(() => columnGenerator).IsNotNull();

    //        _dbClient = dbClient;
    //        _columnGenerator = columnGenerator;
    //        _dataTypeTranslator = new SqlDbDataTypeTranslator();
    //    }

    //    public void Synchronize(IStructureSchema structureSchema)
    //    {
    //        var tableName = structureSchema.GetIndexesTableName();

    //        var schemaChanges = GetSchemaChanges(structureSchema);
    //        if (schemaChanges.Count() < 1)
    //            return;

    //        var columnsToDrop = schemaChanges
    //            .Where(sc => sc.Change == SchemaChanges.IsObsoleteColumn)
    //            .ToList();
    //        DropColumns(tableName, columnsToDrop);

    //        var columnsToAdd = schemaChanges
    //            .Where(sc => sc.Change == SchemaChanges.IsMissingColumn)
    //            .ToList();
    //        AddColumns(tableName, columnsToAdd);
    //    }

    //    private void AddColumns(string tableName, IList<SchemaChange> columns)
    //    {
    //        if (columns == null || columns.Count < 1)
    //            return;

    //        var columnsDdl = columns.Select(sc => _columnGenerator.ToSql(sc.Name, sc.DbDataType));
    //        var columnsDdlCombined = string.Join(",", columnsDdl);
    //        var sql = _dbClient.SqlStatements.GetSql("AddColumns")
    //            .Inject(tableName, columnsDdlCombined);

    //        using (var cmd = _dbClient.CreateCommand(CommandType.Text, sql))
    //        {
    //            cmd.ExecuteNonQuery();
    //        }
    //    }

    //    private void DropColumns(string tableName, IList<SchemaChange> columns)
    //    {
    //        if (columns == null || columns.Count < 1)
    //            return;

    //        var columnNames = columns.Select(sc => "[" + sc.Name + "]");
    //        var namesCombined = string.Join(",", columnNames);
    //        var sql = _dbClient.SqlStatements.GetSql("DropColumns")
    //            .Inject(tableName, namesCombined);

    //        using (var cmd = _dbClient.CreateCommand(CommandType.Text, sql))
    //        {
    //            cmd.ExecuteNonQuery();
    //        }
    //    }

    //    private IEnumerable<SchemaChange> GetSchemaChanges(IStructureSchema structureSchema)
    //    {
    //        var changes = new List<SchemaChange>();
    //        var indexAccessors = new List<IIndexAccessor>(structureSchema.IndexAccessors);
    //        var dbColumns = GetIndexesColumns(structureSchema);

    //        for (var c = dbColumns.Count - 1; c > -1; c--)
    //        {
    //            var dbColumn = dbColumns[c];
    //            var existsInBothSchemas = indexAccessors.Any(iac => iac.Path == dbColumn.Name);

    //            if (!existsInBothSchemas)
    //                changes.Add(new SchemaChange(SchemaChanges.IsObsoleteColumn, dbColumn.Name, dbColumn.DbDataType));

    //            dbColumns.RemoveAt(c);
    //            indexAccessors.RemoveAll(iac => iac.Path == dbColumn.Name);
    //        }

    //        var missingColumnsInDb = indexAccessors.Select(
    //            iac => new SchemaChange(SchemaChanges.IsMissingColumn, iac.Path, _dataTypeTranslator.ToDbType(iac)));
    //        changes.AddRange(missingColumnsInDb);

    //        return changes;
    //    }

    //    private IList<DbColumn> GetIndexesColumns(IStructureSchema structureSchema)
    //    {
    //        var dbColumns = _dbClient.GetColumns(structureSchema.GetIndexesTableName(), IndexStorageSchema.Fields.StructureId.Name);

    //        return dbColumns ?? new List<DbColumn>();
    //    }
    //}
}