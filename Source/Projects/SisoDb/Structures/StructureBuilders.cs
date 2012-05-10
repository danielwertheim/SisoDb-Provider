using System;
using EnsureThat;
using NCore;
using PineCone.Serializers;
using PineCone.Structures;
using PineCone.Structures.IdGenerators;
using PineCone.Structures.Schemas;
using SisoDb.Resources;
using SisoDb.Serialization;

namespace SisoDb.Structures
{
    public class StructureBuilders : IStructureBuilders
    {
        protected readonly Func<IJsonSerializer> SerializerFn;

        public Func<IStructureSchema, IStructureIdGenerator> StructureIdGeneratorFn { get; set; }

        public StructureBuilders(Func<IJsonSerializer> serializerFn)
        {
            SerializerFn = serializerFn;
        }

        public virtual IStructureBuilder ForInserts(IStructureSchema structureSchema, IIdentityStructureIdGenerator identityStructureIdGenerator)
		{
			Ensure.That(structureSchema, "structureSchema").IsNotNull();
			Ensure.That(identityStructureIdGenerator, "identityStructureIdGenerator").IsNotNull();

            var idType = structureSchema.IdAccessor.IdType;

            if (idType == StructureIdTypes.Guid)
                return new StructureBuilder
                {
                    StructureIdGenerator = new SequentialGuidStructureIdGenerator(),
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

        public virtual IStructureBuilder ForUpdates(IStructureSchema structureSchema)
        {
            return new StructureBuilderPreservingId
            {
                StructureIdGenerator = new EmptyStructureIdGenerator(),
                StructureSerializer = GetStructureSerializer()
            };
        }

        protected virtual IStructureSerializer GetStructureSerializer()
        {
            return new SerializerForStructureBuilder(SerializerFn());
        }
    }
}