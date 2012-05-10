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

        public virtual DbConfiguration ResolveGuidStructureIdGeneratorBy(Func<IStructureIdGenerator> fn)
        {
            Database.StructureBuilders.GuidStructureIdGeneratorFn = fn;

            return this;
        }

        public virtual DbConfiguration UseSerializerOf(ISisoDbSerializer serializer)
        {
            Database.Serializer = serializer;

            return this;
        }
    }
}