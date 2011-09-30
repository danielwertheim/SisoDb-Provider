using System.Data;
using SisoDb.Core;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.Sql2008.Dac;
using SisoDb.Structures.Schemas;

namespace SisoDb.Sql2008.DbSchema
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
            var sql = _dbClient.SqlStatements.GetSql("DropStructureTables").Inject(
                structureSchema.GetIndexesTableName(),
                structureSchema.GetUniquesTableName(),
                structureSchema.GetStructureTableName());

            using (var cmd = _dbClient.CreateCommand(CommandType.Text, sql, new DacParameter("entityHash", structureSchema.Hash)))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }
}