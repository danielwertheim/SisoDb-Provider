using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using NCore;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Providers;
using SisoDb.Structures;

namespace SisoDb.DbSchema
{
    public class SqlDbIndexesSchemaSynchronizer : IDbSchemaSynchronizer
    {
        private readonly ISqlStatements _sqlStatements;

        public SqlDbIndexesSchemaSynchronizer(ISqlStatements sqlStatements)
        {
        	Ensure.That(sqlStatements, "sqlStatements").IsNotNull();
			
			_sqlStatements = sqlStatements;
        }

        public void Synchronize(IStructureSchema structureSchema, IDbClient dbClient)
        {
            var keyNamesToDrop = GetKeyNamesToDrop(structureSchema, dbClient);

            if (keyNamesToDrop.Count > 0)
                DeleteRecordsMatchingKeyNames(structureSchema, keyNamesToDrop, dbClient);
        }

		private void DeleteRecordsMatchingKeyNames(IStructureSchema structureSchema, IEnumerable<string> names, IDbClient dbClient)
        {
            var inString = string.Join(",", names.Select(n => "'" + n + "'"));
            var sql = _sqlStatements.GetSql("IndexesSchemaSynchronizer_DeleteRecordsMatchingKeyNames")
                .Inject(structureSchema.GetIndexesTableName(), IndexStorageSchema.Fields.MemberPath.Name, inString);

            dbClient.ExecuteNonQuery(sql);
        }

		private IList<string> GetKeyNamesToDrop(IStructureSchema structureSchema, IDbClient dbClient)
        {
            var structureFields = new HashSet<string>(structureSchema.IndexAccessors.Select(iac => iac.Path));
            var keyNames = GetKeyNames(structureSchema, dbClient);

            return keyNames.Where(kn => !structureFields.Contains(kn)).ToList();
        }

		private IEnumerable<string> GetKeyNames(IStructureSchema structureSchema, IDbClient dbClient)
        {
            var dbColumns = new List<string>();

            dbClient.SingleResultSequentialReader(
                _sqlStatements.GetSql("IndexesSchemaSynchronizer_GetKeyNames").Inject(
                    IndexStorageSchema.Fields.MemberPath.Name,
                    structureSchema.GetIndexesTableName()),
                    dr => dbColumns.Add(dr.GetString(0)));

            return dbColumns;
        }
    }
}