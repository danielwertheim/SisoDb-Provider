using System.Collections.Generic;
using EnsureThat;
using PineCone.Structures.Schemas;
using SisoDb.Dac;

namespace SisoDb.DbSchema
{
	public class SqlDbSchemaUpserter : IDbSchemaUpserter
	{
	    private readonly ISisoDatabase _db;
	    private readonly SqlDbStructuresSchemaBuilder _structuresDbSchemaBuilder;
		private readonly SqlDbIndexesSchemaBuilder _indexesDbSchemaBuilder;
		private readonly SqlDbUniquesSchemaBuilder _uniquesDbSchemaBuilder;

		private readonly SqlDbIndexesSchemaSynchronizer _indexesDbSchemaSynchronizer;
		private readonly SqlDbUniquesSchemaSynchronizer _uniquesDbSchemaSynchronizer;

		public SqlDbSchemaUpserter(ISisoDatabase db, ISqlStatements sqlStatements)
		{
		    Ensure.That(db, "db").IsNotNull();
		    Ensure.That(sqlStatements, "sqlStatements").IsNotNull();

            _db = db;
			_structuresDbSchemaBuilder = new SqlDbStructuresSchemaBuilder(sqlStatements);
			_indexesDbSchemaBuilder = new SqlDbIndexesSchemaBuilder(sqlStatements);
			_uniquesDbSchemaBuilder = new SqlDbUniquesSchemaBuilder(sqlStatements);

			_indexesDbSchemaSynchronizer = new SqlDbIndexesSchemaSynchronizer(sqlStatements);
			_uniquesDbSchemaSynchronizer = new SqlDbUniquesSchemaSynchronizer(sqlStatements);
		}

		public void Upsert(IStructureSchema structureSchema, IDbClient dbClient)
		{
		    var modelInfo = dbClient.GetModelTablesInfo(structureSchema);

            if(_db.Settings.SynchronizeSchemaChanges)
            {
                _indexesDbSchemaSynchronizer.Synchronize(structureSchema, dbClient, modelInfo.GetIndexesTableNamesForExisting());

                if (modelInfo.Statuses.UniquesTableExists)
                    _uniquesDbSchemaSynchronizer.Synchronize(structureSchema, dbClient);
            }

			if (modelInfo.Statuses.AllExists)
				return;

			foreach (var sql in GenerateSql(structureSchema, modelInfo))
				dbClient.ExecuteNonQuery(sql, new DacParameter("entityName", structureSchema.Name));
		}

        private IEnumerable<string> GenerateSql(IStructureSchema structureSchema, ModelTablesInfo modelInfo)
		{
			if (!modelInfo.Statuses.StructureTableExists)
				yield return _structuresDbSchemaBuilder.GenerateSql(structureSchema);

            if (!modelInfo.Statuses.UniquesTableExists)
				yield return _uniquesDbSchemaBuilder.GenerateSql(structureSchema);

            if (!modelInfo.Statuses.IndexesTableStatuses.AllExists)
				foreach (var sql in _indexesDbSchemaBuilder.GenerateSql(structureSchema, modelInfo.Names.IndexesTableNames, modelInfo.Statuses.IndexesTableStatuses))
					yield return sql;
		}
	}
}