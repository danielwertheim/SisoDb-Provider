using System.Text;
using EnsureThat;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Structures;

namespace SisoDb.DbSchema
{
    public class SqlDbSchemaUpserter : IDbSchemaUpserter
    {
        private readonly SqlDbStructuresSchemaBuilder _structuresDbSchemaBuilder;
        private readonly SqlDbIndexesSchemaBuilder _indexesDbSchemaBuilder;
        private readonly SqlDbUniquesSchemaBuilder _uniquesDbSchemaBuilder;

        private readonly SqlDbIndexesSchemaSynchronizer _indexesDbSchemaSynchronizer;
        private readonly SqlDbUniquesSchemaSynchronizer _uniquesDbSchemaSynchronizer;

		public SqlDbSchemaUpserter(ISqlStatements sqlStatements)
		{
			Ensure.That(sqlStatements, "sqlStatements").IsNotNull();

            _structuresDbSchemaBuilder = new SqlDbStructuresSchemaBuilder(sqlStatements);
            _indexesDbSchemaBuilder = new SqlDbIndexesSchemaBuilder(sqlStatements);
            _uniquesDbSchemaBuilder = new SqlDbUniquesSchemaBuilder(sqlStatements);

            _indexesDbSchemaSynchronizer = new SqlDbIndexesSchemaSynchronizer(sqlStatements);
            _uniquesDbSchemaSynchronizer = new SqlDbUniquesSchemaSynchronizer(sqlStatements);
        }

		public void Upsert(IStructureSchema structureSchema, IDbClient dbClient)
        {
            var structuresTableName = structureSchema.GetStructureTableName();
            var indexesTableName = structureSchema.GetIndexesTableName();
            var uniquesTableName = structureSchema.GetUniquesTableName();

            var structuresTableExists = dbClient.TableExists(structuresTableName);
            var indexesTableExists = dbClient.TableExists(indexesTableName);
            var uniquesTableExists = dbClient.TableExists(uniquesTableName);

            if(indexesTableExists)
                _indexesDbSchemaSynchronizer.Synchronize(structureSchema, dbClient);

            if(uniquesTableExists)
                _uniquesDbSchemaSynchronizer.Synchronize(structureSchema, dbClient);

            if (structuresTableExists && indexesTableExists && uniquesTableExists)
                return;

            dbClient.ExecuteNonQuery(GenerateSql(structureSchema, structuresTableExists, indexesTableExists, uniquesTableExists),
                new DacParameter("entityHash", structureSchema.Hash),
                new DacParameter("entityName", structureSchema.Name));
        }

        private string GenerateSql(IStructureSchema structureSchema, bool structuresTableExists, bool indexesTableExists, bool uniquesTableExists)
        {
            var structuresSql = structuresTableExists ? "" : _structuresDbSchemaBuilder.GenerateSql(structureSchema);
            var indexesSql = indexesTableExists ? "" : _indexesDbSchemaBuilder.GenerateSql(structureSchema);
            var uniquesSql = uniquesTableExists ? "" : _uniquesDbSchemaBuilder.GenerateSql(structureSchema);

            var sql = new StringBuilder();

            if (!structuresTableExists)
                sql.AppendLine(structuresSql);

            if (!indexesTableExists)
                sql.AppendLine(indexesSql);

            if (!uniquesTableExists)
                sql.AppendLine(uniquesSql);

            return sql.ToString();
        }
    }
}