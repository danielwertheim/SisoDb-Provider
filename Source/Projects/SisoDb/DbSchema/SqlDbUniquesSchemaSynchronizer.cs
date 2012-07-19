using System.Collections.Generic;
using System.Linq;
using SisoDb.Dac;
using SisoDb.EnsureThat;
using SisoDb.NCore;
using SisoDb.PineCone.Structures.Schemas;

namespace SisoDb.DbSchema
{
    public class SqlDbUniquesSchemaSynchronizer
    {
        private readonly ISqlStatements _sqlStatements;

        public SqlDbUniquesSchemaSynchronizer(ISqlStatements sqlStatements)
        {
        	Ensure.That(sqlStatements, "sqlStatements").IsNotNull();

            _sqlStatements = sqlStatements;
        }

		public void Synchronize(IStructureSchema structureSchema, IDbClient dbClient)
        {
            var keyNamesToDrop = GetDbKeyNamesToDrop(structureSchema, dbClient);

            if (keyNamesToDrop.Any())
                DeleteRecordsMatchingKeyNames(structureSchema, keyNamesToDrop, dbClient);
        }

		private void DeleteRecordsMatchingKeyNames(IStructureSchema structureSchema, IEnumerable<string> names, IDbClient dbClient)
        {
            var inString = string.Join(",", names.Select(n => "'" + n + "'"));
            var sql = _sqlStatements.GetSql("UniquesSchemaSynchronizer_DeleteRecordsMatchingKeyNames")
                .Inject(structureSchema.GetUniquesTableName(), UniqueStorageSchema.Fields.UqMemberPath.Name, inString);

            dbClient.ExecuteNonQuery(sql);
        }

		private string[] GetDbKeyNamesToDrop(IStructureSchema structureSchema, IDbClient dbClient)
        {
            var structureFields = new HashSet<string>(structureSchema.IndexAccessors.Select(iac => iac.Path));
            var keyNames = GetExistingDbKeyNames(structureSchema, dbClient);

            return keyNames.Where(kn => !structureFields.Contains(kn)).ToArray();
        }

		private IEnumerable<string> GetExistingDbKeyNames(IStructureSchema structureSchema, IDbClient dbClient)
        {
            var dbColumns = new List<string>();

            dbClient.SingleResultSequentialReader(
                _sqlStatements.GetSql("UniquesSchemaSynchronizer_GetKeyNames").Inject(
                    UniqueStorageSchema.Fields.UqMemberPath.Name,
                    structureSchema.GetUniquesTableName()),
                    dr => dbColumns.Add(dr.GetString(0)));

            return dbColumns;
        }
    }
}