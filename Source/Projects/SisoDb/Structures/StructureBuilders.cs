using System;
using PineCone.Serializers;
using PineCone.Structures;
using PineCone.Structures.IdGenerators;
using PineCone.Structures.Schemas;
using SisoDb.EnsureThat;
using SisoDb.NCore;
using SisoDb.Resources;
using SisoDb.Serialization;

namespace SisoDb.Structures
{
    public class StructureBuilders : IStructureBuilders
    {
        protected readonly Func<ISisoDbSerializer> SerializerFn;

        public Func<IStructureIdGenerator> GuidStructureIdGeneratorFn { get; set; }
        public StructureBuilderFactoryForInserts ForInserts { get; set; }
        public StructureBuilderFactoryForUpdates ForUpdates { get; set; }

        public StructureBuilders(Func<ISisoDbSerializer> serializerFn)
        {
            Ensure.That(serializerFn, "serializerFn").IsNotNull();

            SerializerFn = serializerFn;
            GuidStructureIdGeneratorFn = GetDefaultGuidStructureIdGenerator;
            ForInserts = GetDefaultBuilderForInserts;
            ForUpdates = GetDefaultBuilderForUpdates;
        }

        protected virtual IStructureIdGenerator GetDefaultGuidStructureIdGenerator()
        {
            return new SequentialGuidStructureIdGenerator();
        }

        protected virtual IStructureBuilder GetDefaultBuilderForInserts(IStructureSchema structureSchema, IIdentityStructureIdGenerator identityStructureIdGenerator)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();
            Ensure.That(identityStructureIdGenerator, "identityStructureIdGenerator").IsNotNull();

            var idType = structureSchema.IdAccessor.IdType;

            if (idType == StructureIdTypes.Guid)
                return new StructureBuilder
                {
                    StructureIdGenerator = GuidStructureIdGeneratorFn.Invoke(),
                    StructureSerializer = GetStructureSerializer()
                };

            if (idType.IsIdentity())
                return new StructureBuilder
                {
                    StructureIdGenerator = identityStructureIdGenerator,
                    StructureSerializer = GetStructureSerializer()
                };

            if (idType == StructureIdTypes.String)
                return new StructureBuilderPreservingId
                {
                    StructureIdGenerator = new EmptyStructureIdGenerator(),
                    StructureSerializer = GetStructureSerializer()
                };

            throw new SisoDbException(ExceptionMessages.StructureBuilders_CreateForInsert.Inject(idType, structureSchema.Name));
        }

        protected virtual IStructureBuilder GetDefaultBuilderForUpdates(IStructureSchema structureSchema)
        {
            return new StructureBuilderPreservingId
            {
                StructureIdGenerator = new EmptyStructureIdGenerator(),
                StructureSerializer = GetStructureSerializer()
            };
        }

        protected virtual IStructureSerializer GetStructureSerializer()
        {
            return new StructureSerializer(SerializerFn());
        }

        public static IStructureBuilder ForPreservingStructureIds(ISisoDbSerializer serializer)
        {
            return new StructureBuilderPreservingId
            {
                StructureIdGenerator = new EmptyStructureIdGenerator(),
                StructureSerializer = new StructureSerializer(serializer)
            };
        }
    }
}