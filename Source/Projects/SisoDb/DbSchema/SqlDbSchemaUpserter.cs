using System;
using System.Collections.Generic;
using SisoDb.Dac;
using SisoDb.EnsureThat;
using SisoDb.PineCone.Structures.Schemas;

namespace SisoDb.DbSchema
{
    public class SqlDbSchemaUpserter : IDbSchemaUpserter
    {
        protected readonly ISisoDatabase Db;
        protected readonly SqlDbStructuresSchemaBuilder StructuresDbSchemaBuilder;
        protected readonly SqlDbIndexesSchemaBuilder IndexesDbSchemaBuilder;
        protected readonly SqlDbUniquesSchemaBuilder UniquesDbSchemaBuilder;

        protected readonly SqlDbIndexesSchemaSynchronizer IndexesDbSchemaSynchronizer;
        protected readonly SqlDbUniquesSchemaSynchronizer UniquesDbSchemaSynchronizer;

        public SqlDbSchemaUpserter(ISisoDatabase db, ISqlStatements sqlStatements)
        {
            Ensure.That(db, "db").IsNotNull();
            Ensure.That(sqlStatements, "sqlStatements").IsNotNull();

            Db = db;
            StructuresDbSchemaBuilder = new SqlDbStructuresSchemaBuilder(sqlStatements);
            IndexesDbSchemaBuilder = new SqlDbIndexesSchemaBuilder(sqlStatements);
            UniquesDbSchemaBuilder = new SqlDbUniquesSchemaBuilder(sqlStatements);

            IndexesDbSchemaSynchronizer = new SqlDbIndexesSchemaSynchronizer(sqlStatements);
            UniquesDbSchemaSynchronizer = new SqlDbUniquesSchemaSynchronizer(sqlStatements);
        }

        public virtual void Upsert(IStructureSchema structureSchema, Func<IDbClient> dbClientFn)
        {
            if (!Db.Settings.AllowUpsertsOfSchemas)
                return;

            var dbClient = dbClientFn();
            var modelInfo = dbClient.GetModelTablesInfo(structureSchema);

            if (Db.Settings.SynchronizeSchemaChanges)
            {
                IndexesDbSchemaSynchronizer.Synchronize(structureSchema, dbClient, modelInfo.GetIndexesTableNamesForExisting());

                if (modelInfo.Statuses.UniquesTableExists)
                    UniquesDbSchemaSynchronizer.Synchronize(structureSchema, dbClient);
            }

            if (modelInfo.Statuses.AllExists)
                return;

            foreach (var sql in GenerateSql(structureSchema, modelInfo))
                dbClient.ExecuteNonQuery(sql, new DacParameter(DbSchemas.Parameters.EntityNameParamPrefix, structureSchema.Name));
        }

        protected virtual IEnumerable<string> GenerateSql(IStructureSchema structureSchema, ModelTablesInfo modelInfo)
        {
            if (!modelInfo.Statuses.StructureTableExists)
                yield return StructuresDbSchemaBuilder.GenerateSql(structureSchema);

            if (!modelInfo.Statuses.UniquesTableExists)
                yield return UniquesDbSchemaBuilder.GenerateSql(structureSchema);

            if (!modelInfo.Statuses.IndexesTableStatuses.AllExists)
                foreach (var sql in IndexesDbSchemaBuilder.GenerateSql(structureSchema, modelInfo.Names.IndexesTableNames, modelInfo.Statuses.IndexesTableStatuses))
                    yield return sql;
        }
    }
}