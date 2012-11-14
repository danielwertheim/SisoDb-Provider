using System;
using SisoDb.Dac;
using SisoDb.EnsureThat;
using SisoDb.Serialization;
using SisoDb.Structures.IdGenerators;
using SisoDb.Structures.Schemas;

namespace SisoDb.Structures
{
    public class StructureBuilders : IStructureBuilders
    {
        public Func<IStructureSchema, IStructureIdGenerator> GuidStructureIdGeneratorFn { get; set; }
        public Func<IStructureSchema, IDbClient, IIdentityStructureIdGenerator> IdentityStructureIdGeneratorFn { get; set; }
        public Func<ISisoSerializer> SerializerFn { get; private set; }
        public Func<IStructureSerializer> StructureSerializerFn { get; set; }
        public Func<IStructureSchema, IDbClient, IStructureBuilder> ResolveBuilderForInsertsBy { get; set; }
        public Func<IStructureSchema, IStructureBuilder> ResolveBuilderForUpdatesBy { get; set; }
        
        public StructureBuilders(
            Func<ISisoSerializer> serializerFn, 
            Func<IStructureSchema, IStructureIdGenerator> guidStructureIdGenerator, 
            Func<IStructureSchema, IDbClient, IIdentityStructureIdGenerator> identityStructureIdGeneratorFn)
        {
            Ensure.That(serializerFn, "serializerFn").IsNotNull();
            Ensure.That(guidStructureIdGenerator, "guidStructureIdGenerator").IsNotNull();
            Ensure.That(identityStructureIdGeneratorFn, "identityStructureIdGeneratorFn").IsNotNull();

            GuidStructureIdGeneratorFn = guidStructureIdGenerator;
            IdentityStructureIdGeneratorFn = identityStructureIdGeneratorFn;
            SerializerFn = serializerFn;
            StructureSerializerFn = () => new StructureSerializer(SerializerFn());
            ResolveBuilderForInsertsBy = GetDefaultBuilderForInserts;
            ResolveBuilderForUpdatesBy = GetDefaultBuilderForUpdates;
        }

        protected virtual IStructureBuilder GetDefaultBuilderForInserts(IStructureSchema structureSchema, IDbClient dbClient)
        {
            return StructureBuildersFn.GetDefaultBuilderForInserts(this, structureSchema, dbClient);
        }

        protected virtual IStructureBuilder GetDefaultBuilderForUpdates(IStructureSchema structureSchema)
        {
            return new StructureBuilderPreservingId
            {
                StructureIdGenerator = new EmptyStructureIdGenerator(),
                StructureSerializer = StructureSerializerFn()
            };
        }

        public virtual IStructureBuilder ResolveBuilderForInsert(IStructureSchema structureSchema, IDbClient dbClient)
        {
            return ResolveBuilderForInsertsBy(structureSchema, dbClient);
        }

        public virtual IStructureBuilder ResolveBuilderForUpdate(IStructureSchema structureSchema)
        {
            return ResolveBuilderForUpdatesBy(structureSchema);
        }
    }
}