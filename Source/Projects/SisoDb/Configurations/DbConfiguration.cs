using System;
using SisoDb.EnsureThat;
using SisoDb.PineCone.Structures;
using SisoDb.PineCone.Structures.Schemas.Configuration;
using SisoDb.Serialization;
using SisoDb.Structures;

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

        public virtual DbConfiguration UseManualStructureIdAssignment()
        {
            Database.StructureBuilders.ForInserts = (schema, generator) => StructureBuilders.ForPreservingStructureIds(Database.Serializer);

            return this;
        }

        public virtual DbConfiguration UseGuidStructureIdGeneratorResolvedBy(Func<IStructureIdGenerator> fn)
        {
            Database.StructureBuilders.GuidStructureIdGeneratorFn = fn;

            return this;
        }

        public virtual DbConfiguration UseSerializerResolvedBy(Func<ISisoDbSerializer> fn)
        {
            Database.Serializer = fn();

            return this;
        }

        public virtual DbConfiguration UseCacheProviderResolvedBy(Func<ICacheProvider> fn)
        {
            Database.CacheProvider = fn();

            return this;
        }

        public virtual DbConfiguration ForProduction()
        {
            Database.Settings.AllowUpsertsOfSchemas = false;
            Database.Settings.SynchronizeSchemaChanges = false;

            return this;
        }

        public virtual DbConfiguration Serializer(Action<SerializerOptions> config)
        {
            config(Database.Serializer.Options);

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