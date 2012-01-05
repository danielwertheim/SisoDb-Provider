using System.Collections.Generic;
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

			foreach (var sql in GenerateSql(structureSchema, structuresTableExists, indexesTableStatuses, uniquesTableExists))
			{
				dbClient.ExecuteNonQuery(sql, 
					new DacParameter("entityHash", structureSchema.Hash), 
					new DacParameter("entityName", structureSchema.Name));
			}
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

		private IEnumerable<string> GenerateSql(IStructureSchema structureSchema, bool structuresTableExists, IndexesTableStatuses indexesTableStatuses, bool uniquesTableExists)
		{
			if (!structuresTableExists)
				yield return _structuresDbSchemaBuilder.GenerateSql(structureSchema);

			if(!uniquesTableExists)
				yield return _uniquesDbSchemaBuilder.GenerateSql(structureSchema);
			
			if(!indexesTableStatuses.AllExists)
				foreach (var sql in _indexesDbSchemaBuilder.GenerateSql(structureSchema, indexesTableStatuses))
					yield return sql;
		}
	}
}