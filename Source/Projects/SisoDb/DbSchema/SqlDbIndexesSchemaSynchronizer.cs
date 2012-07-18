using System.Collections.Generic;
using System.Linq;
using NCore;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.EnsureThat;

namespace SisoDb.DbSchema
{
    public class SqlDbIndexesSchemaSynchronizer
    {
        private readonly ISqlStatements _sqlStatements;

        public SqlDbIndexesSchemaSynchronizer(ISqlStatements sqlStatements)
        {
        	Ensure.That(sqlStatements, "sqlStatements").IsNotNull();
			
			_sqlStatements = sqlStatements;
        }

        public void Synchronize(IStructureSchema structureSchema, IDbClient dbClient, string[] indexesTableNames)
        {
            if(!indexesTableNames.Any())
                return;

			var structureFields = new HashSet<string>(structureSchema.IndexAccessors.Select(iac => iac.Path));
			foreach (var indexesTableName in indexesTableNames)
        	{
				var keyNamesToDrop = GetMemberPathsToDrop(indexesTableName, structureFields, dbClient);

				if (keyNamesToDrop.Length > 0)
					DeleteRecordsMatchingKeyNames(indexesTableName, keyNamesToDrop, dbClient);	
        	}
        }

		private void DeleteRecordsMatchingKeyNames(string indexesTableName, IEnumerable<string> names, IDbClient dbClient)
        {
            var inString = string.Join(",", names.Select(n => "'" + n + "'"));
            var sql = _sqlStatements.GetSql("IndexesSchemaSynchronizer_DeleteRecordsMatchingKeyNames")
                .Inject(indexesTableName, IndexStorageSchema.Fields.MemberPath.Name, inString);

            dbClient.ExecuteNonQuery(sql);
        }

		private string[] GetMemberPathsToDrop(string indexesTableName, HashSet<string> structureFields, IDbClient dbClient)
        {
			return GetExistingDbMemberPaths(indexesTableName, dbClient).Where(kn => !structureFields.Contains(kn)).ToArray();
        }

		private IEnumerable<string> GetExistingDbMemberPaths(string indexesTableName, IDbClient dbClient)
        {
            var dbColumns = new List<string>();

            dbClient.SingleResultSequentialReader(
                _sqlStatements.GetSql("IndexesSchemaSynchronizer_GetKeyNames").Inject(
                    IndexStorageSchema.Fields.MemberPath.Name,
                    indexesTableName),
                    dr => dbColumns.Add(dr.GetString(0)));

            return dbColumns;
        }
    }
}