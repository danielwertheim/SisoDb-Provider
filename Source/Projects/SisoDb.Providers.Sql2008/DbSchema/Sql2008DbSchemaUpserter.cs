using System.Data;
using System.Text;
using EnsureThat;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Providers;

namespace SisoDb.Sql2008.DbSchema
{
    public class Sql2008DbSchemaUpserter : IDbSchemaUpserter
    {
        private readonly SqlDbStructuresSchemaBuilder _structuresDbSchemaBuilder;
        private readonly SqlDbIndexesSchemaBuilder _indexesDbSchemaBuilder;
        private readonly SqlDbUniquesSchemaBuilder _uniquesDbSchemaBuilder;

        private readonly SqlDbIndexesSchemaSynchronizer _indexesDbSchemaSynchronizer;
        private readonly SqlDbUniquesSchemaSynchronizer _uniquesDbSchemaSynchronizer;
        private readonly IDbClient _dbClient;

        public Sql2008DbSchemaUpserter(IDbClient dbClient)
        {
            Ensure.That(() => dbClient).IsNotNull();

            _dbClient = dbClient;

            var columnGenerator =
                SisoEnvironment.ProviderFactories.Get(dbClient.ProviderType).GetDbColumnGenerator();

            _structuresDbSchemaBuilder = new SqlDbStructuresSchemaBuilder(_dbClient.SqlStatements);
            _indexesDbSchemaBuilder = new SqlDbIndexesSchemaBuilder(_dbClient.SqlStatements, columnGenerator);
            _uniquesDbSchemaBuilder = new SqlDbUniquesSchemaBuilder(_dbClient.SqlStatements);

            _indexesDbSchemaSynchronizer = new SqlDbIndexesSchemaSynchronizer(_dbClient);
            _uniquesDbSchemaSynchronizer = new SqlDbUniquesSchemaSynchronizer(_dbClient);
        }

        public void Upsert(IStructureSchema structureSchema)
        {
            var structuresTableName = structureSchema.GetStructureTableName();
            var indexesTableName = structureSchema.GetIndexesTableName();
            var uniquesTableName = structureSchema.GetUniquesTableName();

            var structuresTableExists = _dbClient.TableExists(structuresTableName);
            var indexesTableExists = _dbClient.TableExists(indexesTableName);
            var uniquesTableExists = _dbClient.TableExists(uniquesTableName);

            if (structuresTableExists && indexesTableExists && uniquesTableExists)
            {
                _indexesDbSchemaSynchronizer.Synchronize(structureSchema);
                _uniquesDbSchemaSynchronizer.Synchronize(structureSchema);
                return;
            }

            var structuresSql = structuresTableExists ? "" : _structuresDbSchemaBuilder.GenerateSql(structureSchema);
            var indexesSql = indexesTableExists ? "" : _indexesDbSchemaBuilder.GenerateSql(structureSchema);
            var uniquesSql = uniquesTableExists ? "" : _uniquesDbSchemaBuilder.GenerateSql(structureSchema);

            using (var cmd = _dbClient.CreateCommand(CommandType.Text, null,
                new DacParameter("entityHash", structureSchema.Hash),
                new DacParameter("entityName", structureSchema.Name)))
            {
                var sql = new StringBuilder();

                if (!structuresTableExists)
                    sql.AppendLine(structuresSql);

                if (!indexesTableExists)
                    sql.AppendLine(indexesSql);
                else
                    _indexesDbSchemaSynchronizer.Synchronize(structureSchema);

                if (!uniquesTableExists)
                    sql.AppendLine(uniquesSql);
                else
                    _uniquesDbSchemaSynchronizer.Synchronize(structureSchema);

                cmd.CommandText = sql.ToString();
                cmd.ExecuteNonQuery();
            }
        }
    }
}