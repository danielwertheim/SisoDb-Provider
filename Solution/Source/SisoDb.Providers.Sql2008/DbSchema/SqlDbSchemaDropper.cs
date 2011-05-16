using System.Data;
using SisoDb.Core;
using SisoDb.Providers.DbSchema;
using SisoDb.Providers.Sql2008.Dac;
using SisoDb.Querying;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.Sql2008.DbSchema
{
    public class SqlDbSchemaDropper : IDbSchemaDropper
    {
        private readonly SqlDbClient _dbClient;

        public SqlDbSchemaDropper(SqlDbClient dbClient)
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