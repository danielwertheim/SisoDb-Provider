using System;
using SisoDb.Dac;
using SisoDb.EnsureThat;
using SisoDb.NCore;
using SisoDb.Resources;
using SisoDb.Serialization;
using SisoDb.Structures.IdGenerators;
using SisoDb.Structures.Schemas;

namespace SisoDb.Structures
{
    public class StructureBuilders : IStructureBuilders
    {
        protected readonly Func<ISisoDbSerializer> SerializerFn;

        public Func<IStructureSchema, IStructureIdGenerator> GuidStructureIdGeneratorFn { get; set; }
        public Func<IStructureSchema, IDbClient, IIdentityStructureIdGenerator> IdentityStructureIdGeneratorFn { get; set; }
        public Func<IStructureSchema, IDbClient, IStructureBuilder> ForInserts { get; set; }
        public Func<IStructureSchema, IStructureBuilder> ForUpdates { get; set; }

        public StructureBuilders(
            Func<ISisoDbSerializer> serializerFn, 
            Func<IStructureSchema, IStructureIdGenerator> guidStructureIdGenerator, 
            Func<IStructureSchema, IDbClient, IIdentityStructureIdGenerator> identityStructureIdGeneratorFn)
        {
            Ensure.That(serializerFn, "serializerFn").IsNotNull();
            Ensure.That(guidStructureIdGenerator, "guidStructureIdGenerator").IsNotNull();
            Ensure.That(identityStructureIdGeneratorFn, "identityStructureIdGeneratorFn").IsNotNull();

            SerializerFn = serializerFn;
            GuidStructureIdGeneratorFn = guidStructureIdGenerator;
            IdentityStructureIdGeneratorFn = identityStructureIdGeneratorFn;
            ForInserts = GetDefaultBuilderForInserts;
            ForUpdates = GetDefaultBuilderForUpdates;
        }

        protected virtual IStructureBuilder GetDefaultBuilderForInserts(IStructureSchema structureSchema, IDbClient dbClient)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var idType = structureSchema.IdAccessor.IdType;
            if (idType == StructureIdTypes.Guid)
                return new StructureBuilder
                {
                    StructureIdGenerator = GuidStructureIdGeneratorFn(structureSchema),
                    StructureSerializer = GetStructureSerializer()
                };

            if (idType.IsIdentity())
                return new StructureBuilder
                {
                    StructureIdGenerator = IdentityStructureIdGeneratorFn(structureSchema, dbClient),
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
    }
}