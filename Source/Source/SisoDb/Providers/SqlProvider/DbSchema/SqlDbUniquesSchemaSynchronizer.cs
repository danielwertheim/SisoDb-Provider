using System.Collections.Generic;
using System.Data;
using System.Linq;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    /// <summary>
    /// Delete records that represents an unique for a column that
    /// has been dropped in the key-value table for Uniques.
    /// </summary>
    internal class SqlDbUniquesSchemaSynchronizer : ISqlDbSchemaSynchronizer
    {
        private readonly SqlDbClient _dbClient;
        private readonly ISqlStrings _sqlStrings;

        internal SqlDbUniquesSchemaSynchronizer(SqlDbClient dbClient)
        {
            _dbClient = dbClient;
            _sqlStrings = dbClient.SqlStrings;
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
            var sql = _sqlStrings.GetSql("UniquesSchemaSynchronizer_DeleteRecordsMatchingKeyNames")
                .Inject(structureSchema.GetUniquesTableName(), UniqueStorageSchema.Fields.Name.Name, inString);

            _dbClient.ExecuteNonQuery(CommandType.Text, sql);
        }

        private IList<string> GetKeyNamesToDrop(IStructureSchema structureSchema)
        {
            var structureFields = new HashSet<string>(structureSchema.IndexAccessors.Select(iac => iac.Name));
            var keyNames = GetKeyNames(structureSchema);

            return keyNames.Where(kn => !structureFields.Contains(kn)).ToList();
        }

        private IEnumerable<string> GetKeyNames(IStructureSchema structureSchema)
        {
            var dbColumns = new List<string>();

            _dbClient.ExecuteSingleResultReader(CommandType.Text,
                                                _sqlStrings.GetSql("UniquesSchemaSynchronizer_GetKeyNames")
                                                .Inject(
                                                    UniqueStorageSchema.Fields.Name.Name, structureSchema.GetUniquesTableName()),
                                                    dr => dbColumns.Add(dr.GetString(0)));

            return dbColumns;
        }
    }
}