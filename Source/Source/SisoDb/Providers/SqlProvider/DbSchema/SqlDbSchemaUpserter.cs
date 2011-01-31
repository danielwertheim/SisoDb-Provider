using System.Data;
using SisoDb.Querying;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    internal class SqlDbSchemaUpserter
    {
        private readonly SqlDbStructuresSchemaBuilder _structuresDbSchemaBuilder;
        private readonly SqlDbIndexesSchemaBuilder _indexesDbSchemaBuilder;
        private readonly SqlDbUniquesSchemaBuilder _uniquesDbSchemaBuilder;

        private readonly SqlDbIndexesSchemaSynchronizer _indexesDbSchemaSynchronizer;
        private readonly SqlDbUniquesSchemaSynchronizer _uniquesDbSchemaSynchronizer;
        private readonly SqlDbClient _dbClient;

        public SqlDbSchemaUpserter(SqlDbClient dbClient)
        {
            _dbClient = dbClient;
            _structuresDbSchemaBuilder = new SqlDbStructuresSchemaBuilder(_dbClient.SqlStrings);
            _indexesDbSchemaBuilder = new SqlDbIndexesSchemaBuilder(_dbClient.SqlStrings, _dbClient.ProviderType);
            _uniquesDbSchemaBuilder = new SqlDbUniquesSchemaBuilder(_dbClient.SqlStrings);

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
                new QueryParameter("entityHash", structureSchema.Hash),
                new QueryParameter("entityName", structureSchema.Name)))
            {
                if (!structuresTableExists)
                {
                    cmd.CommandText = structuresSql;
                    cmd.ExecuteNonQuery();
                }

                if (!indexesTableExists)
                {
                    cmd.CommandText = indexesSql;
                    cmd.ExecuteNonQuery();
                }
                else
                    _indexesDbSchemaSynchronizer.Synchronize(structureSchema);

                if (!uniquesTableExists)
                {
                    cmd.CommandText = uniquesSql;
                    cmd.ExecuteNonQuery();
                }
                else
                    _uniquesDbSchemaSynchronizer.Synchronize(structureSchema);
            }
        }
    }
}