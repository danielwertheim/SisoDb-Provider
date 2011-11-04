using System.Collections.Generic;
using System.Data;
using System.Linq;
using NCore;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Providers;
using SisoDb.Structures;

namespace SisoDb.DbSchema
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
            var sql = _sqlStatements.GetSql("IndexesSchemaSynchronizer_DeleteRecordsMatchingKeyNames")
                .Inject(structureSchema.GetIndexesTableName(), IndexStorageSchema.Fields.MemberPath.Name, inString);

            using (var cmd = _dbClient.CreateCommand(sql))
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
                _sqlStatements.GetSql("IndexesSchemaSynchronizer_GetKeyNames").Inject(
                    IndexStorageSchema.Fields.MemberPath.Name,
                    structureSchema.GetIndexesTableName()),
                    dr => dbColumns.Add(dr.GetString(0)));

            return dbColumns;
        }
    }
}