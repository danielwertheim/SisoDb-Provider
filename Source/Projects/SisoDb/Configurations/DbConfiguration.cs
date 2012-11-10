using System;
using SisoDb.EnsureThat;
using SisoDb.Serialization;
using SisoDb.Structures;
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

        /// <summary>
        /// If an Id exists it will be left untouched, otherwise a new ID will be generated.
        /// </summary>
        /// <returns></returns>
        public virtual DbConfiguration UseAutoIds()
        {
            Database.StructureBuilders.ResolveBuilderForInsertsBy = (schema, dbClient) =>
                StructureBuildersFn.GetBuilderForInsertsAssigningIfMissingId(Database.StructureBuilders, schema, dbClient);

            return this;
        }

        /// <summary>
        /// GUID and String Ids should have been assigned. Identities will be generated for you.
        /// </summary>
        /// <returns></returns>
        public virtual DbConfiguration PreserveIds()
        {
            Database.StructureBuilders.ResolveBuilderForInsertsBy = (schema, dbClient) =>
                StructureBuildersFn.GetBuilderForInsertsPreservingId(Database.StructureBuilders, schema, dbClient);

            return this;
        }

        /// <summary>
        /// No Ids will be generated. You are responsible for doing it.
        /// </summary>
        /// <returns></returns>
        public virtual DbConfiguration UseManualIds()
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