using System.Data;
using SisoDb.Core;
using SisoDb.Providers.DbSchema;
using SisoDb.Querying;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    public class SqlDbSchemaDropper : IDbSchemaDropper
    {
        private readonly ISqlDbClient _dbClient;

        public SqlDbSchemaDropper(ISqlDbClient dbClient)
        {
            _dbClient = dbClient.AssertNotNull("dbClient");
        }

        public void Drop(IStructureSchema structureSchema)
        {
            var sql = _dbClient.SqlStringsRepository.GetSql("DropStructureTables").Inject(
                structureSchema.GetIndexesTableName(),
                structureSchema.GetStructureTableName(),
                structureSchema.GetUniquesTableName());

            using (var cmd = _dbClient.CreateCommand(CommandType.Text, sql, new QueryParameter("entityHash", structureSchema.Hash)))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }
}