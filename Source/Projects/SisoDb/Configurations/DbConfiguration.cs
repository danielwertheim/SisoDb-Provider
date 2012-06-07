using System;
using EnsureThat;
using PineCone.Structures;
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
            Database.CacheProvider = fn.Invoke();

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
    }
}