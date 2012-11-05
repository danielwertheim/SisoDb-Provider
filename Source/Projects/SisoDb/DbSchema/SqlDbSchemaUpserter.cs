using System.Collections.Generic;
using SisoDb.Dac;
using SisoDb.EnsureThat;
using SisoDb.PineCone.Structures.Schemas;

namespace SisoDb.DbSchema
{
    public class SqlDbSchemaUpserter : IDbSchemaUpserter
    {
        protected readonly SqlDbStructuresSchemaBuilder StructuresDbSchemaBuilder;
        protected readonly SqlDbIndexesSchemaBuilder IndexesDbSchemaBuilder;
        protected readonly SqlDbUniquesSchemaBuilder UniquesDbSchemaBuilder;

        protected readonly SqlDbIndexesSchemaSynchronizer IndexesDbSchemaSynchronizer;
        protected readonly SqlDbUniquesSchemaSynchronizer UniquesDbSchemaSynchronizer;

        public SqlDbSchemaUpserter(ISqlStatements sqlStatements)
        {
            Ensure.That(sqlStatements, "sqlStatements").IsNotNull();

            StructuresDbSchemaBuilder = new SqlDbStructuresSchemaBuilder(sqlStatements);
            IndexesDbSchemaBuilder = new SqlDbIndexesSchemaBuilder(sqlStatements);
            UniquesDbSchemaBuilder = new SqlDbUniquesSchemaBuilder(sqlStatements);

            IndexesDbSchemaSynchronizer = new SqlDbIndexesSchemaSynchronizer(sqlStatements);
            UniquesDbSchemaSynchronizer = new SqlDbUniquesSchemaSynchronizer(sqlStatements);
        }

        public virtual void Upsert(IStructureSchema structureSchema, IDbClient dbClient, bool allowDynamicSchemaCreation, bool synchronizeSchemaChanges)
        {
            if(!allowDynamicSchemaCreation && !synchronizeSchemaChanges)
                return;

            var modelInfo = dbClient.GetModelTablesInfo(structureSchema);

            if (synchronizeSchemaChanges)
            {
                if(!modelInfo.Statuses.IndexesTableStatuses.AllExists)
                    IndexesDbSchemaSynchronizer.Synchronize(structureSchema, dbClient, modelInfo.GetIndexesTableNamesForExisting());

                if (modelInfo.Statuses.UniquesTableExists)
                    UniquesDbSchemaSynchronizer.Synchronize(structureSchema, dbClient);
            }

            if (!allowDynamicSchemaCreation || modelInfo.Statuses.AllExists)
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