using System;
using SisoDb.EnsureThat;
using SisoDb.Serialization;
using SisoDb.Structures;
using SisoDb.Structures.IdGenerators;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.Configuration;

namespace SisoDb.Configurations
{
    public class DbConfiguration
    {
        public ISisoDatabase Database { get; private set; }

        public DbConfiguration(ISisoDatabase database)
        {
            Ensure.That(database, "database").IsNotNull();
            Database = database;
        }

        public virtual DbConfiguration PreserveIds()
        {
            Database.StructureBuilders.ResolveBuilderForInsertsBy = (schema, dbClient) => 
                StructureBuildersFn.GetBuilderForInsertsPreservingIds(Database.StructureBuilders, schema, dbClient);

            return this;
        }

        public virtual DbConfiguration UseManualStructureIdAssignment()
        {
            Database.StructureBuilders.ResolveBuilderForInsertsBy = (schema, dbClient) => 
                StructureBuildersFn.GetBuilderForManualIdAssignment(Database.StructureBuilders, schema, dbClient);

            return this;
        }

        public virtual DbConfiguration UseGuidStructureIdGeneratorResolvedBy(Func<IStructureSchema, IStructureIdGenerator> fn)
        {
            Database.StructureBuilders.GuidStructureIdGeneratorFn = fn;

            return this;
        }

        public virtual DbConfiguration UseSerializer(Func<ISisoSerializer> fn)
        {
            Database.Serializer = fn();

            return this;
        }

        public virtual DbConfiguration UseCacheProvider(Func<ICacheProvider> fn)
        {
            Database.CacheProvider = fn();

            return this;
        }

        public virtual DbConfiguration ForProduction()
        {
            Database.Settings.AllowDynamicSchemaCreation = false;
            Database.Settings.AllowDynamicSchemaUpdates = false;

            return this;
        }

        public virtual DbConfiguration StructureType(Type structureType, Action<IStructureTypeConfigurator> config)
        {
            Database.StructureSchemas.StructureTypeFactory.Configurations.Configure(structureType, config);

            return this;
        }

        public virtual DbConfiguration StructureType<T>(Action<IStructureTypeConfigurator<T>> config) where T : class 
        {
            Database.StructureSchemas.StructureTypeFactory.Configurations.Configure<T>(config);

            return this;
        }
    }
}