using System.Text;
using EnsureThat;
using PineCone.Structures.Schemas;
using SisoDb.Dac;

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
			var indexesTableNames = structureSchema.GetIndexesTableNames();
			var uniquesTableName = structureSchema.GetUniquesTableName();

			var structuresTableExists = dbClient.TableExists(structuresTableName);
			var indexesTableStatuses = GetIndexesTableStatuses(indexesTableNames, dbClient);
			var uniquesTableExists = dbClient.TableExists(uniquesTableName);

			if (indexesTableStatuses.AllExists)
				_indexesDbSchemaSynchronizer.Synchronize(structureSchema, dbClient);

			if (uniquesTableExists)
				_uniquesDbSchemaSynchronizer.Synchronize(structureSchema, dbClient);

			if (structuresTableExists && indexesTableStatuses.AllExists && uniquesTableExists)
				return;

			dbClient.ExecuteNonQuery(GenerateSql(structureSchema, structuresTableExists, indexesTableStatuses, uniquesTableExists),
				new DacParameter("entityHash", structureSchema.Hash),
				new DacParameter("entityName", structureSchema.Name));
		}

		private static IndexesTableStatuses GetIndexesTableStatuses(IndexesTableNames names, IDbClient dbClient)
		{
			return new IndexesTableStatuses(names)
			{
				IntegersTableExists = dbClient.TableExists(names.IntegersTableName),
				FractalsTableExists = dbClient.TableExists(names.FractalsTableName),
				DatesTableExists = dbClient.TableExists(names.DatesTableName),
				BooleansTableExists = dbClient.TableExists(names.BooleansTableName),
				GuidsTableExists = dbClient.TableExists(names.GuidsTableName),
				StringsTableExists = dbClient.TableExists(names.StringsTableName)
			};
		}

		private string GenerateSql(IStructureSchema structureSchema, bool structuresTableExists, IndexesTableStatuses indexesTableStatuses, bool uniquesTableExists)
		{
			var structuresSql = structuresTableExists ? "" : _structuresDbSchemaBuilder.GenerateSql(structureSchema)[0];
			var indexesSql = indexesTableStatuses.AllExists ? new[] { "" } : _indexesDbSchemaBuilder.GenerateSql(structureSchema);
			var uniquesSql = uniquesTableExists ? "" : _uniquesDbSchemaBuilder.GenerateSql(structureSchema)[0];

			var sql = new StringBuilder();

			if (!structuresTableExists)
				sql.AppendLine(structuresSql);

			if (!indexesTableStatuses.AllExists)
				sql.AppendLine(indexesSql);

			if (!uniquesTableExists)
				sql.AppendLine(uniquesSql);

			return sql.ToString();
		}
	}
}