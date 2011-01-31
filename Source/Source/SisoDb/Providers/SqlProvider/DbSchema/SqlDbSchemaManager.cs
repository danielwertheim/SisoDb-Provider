using System.Collections.Generic;
using System.Data;
using SisoDb.Providers.Sql;
using SisoDb.Querying;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    internal class SqlDbSchemaManager : IDbSchemaManager
    {
        private readonly ISisoConnectionInfo _connectionInfo;
        private readonly ISet<string> _upsertedSchemas;
        private readonly ISqlStrings _sqlStrings;

        internal SqlDbSchemaManager(ISisoConnectionInfo connectionInfo)
        {
            _connectionInfo = connectionInfo;
            _upsertedSchemas = new HashSet<string>();
            _sqlStrings = new SqlStrings(_connectionInfo.ProviderType);
        }

        public void DropStructureSet(IStructureSchema structureSchema)
        {
            lock (_upsertedSchemas)
            {
                _upsertedSchemas.Remove(structureSchema.Name);

                var sql = _sqlStrings.GetSql("DropStructureTables").Inject(
                    structureSchema.GetIndexesTableName(),
                    structureSchema.GetStructureTableName());

                using (var client = new SqlDbClient(_connectionInfo, false))
                {
                    client.ExecuteNonQuery(CommandType.Text, sql, new QueryParameter("entityHash", structureSchema.Hash));
                }
            }
        }

        public void UpsertStructureSet(IStructureSchema structureSchema)
        {
            lock (_upsertedSchemas)
            {
                if (_upsertedSchemas.Contains(structureSchema.Name))
                    return;

                using (var dbClient = new SqlDbClient(_connectionInfo, false))
                {
                    var upserter = new SqlDbSchemaUpserter(dbClient);
                    upserter.Upsert(structureSchema);
                }

                _upsertedSchemas.Add(structureSchema.Name);
            }
        }
    }
}