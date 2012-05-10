using EnsureThat;
using NCore;
using PineCone.Structures;
using PineCone.Structures.IdGenerators;
using PineCone.Structures.Schemas;
using SisoDb.Resources;
using SisoDb.Serialization;

namespace SisoDb.Structures
{
    public delegate IStructureBuilder StructureBuilderFactoryForInserts(IStructureSchema structureSchema, IIdentityStructureIdGenerator identityStructureIdGenerator);
    public delegate IStructureBuilder StructureBuilderFactoryForUpdates(IStructureSchema structureSchema);

    public class StructureBuilders : IStructureBuilders
    {
        public StructureBuilderFactoryForInserts ForInserts { get; set; }
        public StructureBuilderFactoryForUpdates ForUpdates { get; set; }

		public StructureBuilders()
		{
			Reset();
        }

        public void Reset()
        {
            ForInserts = DefaultForInserts;
            ForUpdates = DefaultForUpdates;
        }

        public static IStructureBuilder DefaultForInserts(IStructureSchema structureSchema, IIdentityStructureIdGenerator identityStructureIdGenerator)
		{
			Ensure.That(structureSchema, "structureSchema").IsNotNull();
			Ensure.That(identityStructureIdGenerator, "identityStructureIdGenerator").IsNotNull();

            var idType = structureSchema.IdAccessor.IdType;

            if (idType == StructureIdTypes.Guid)
                return new StructureBuilder
                {
                    StructureIdGenerator = new SequentialGuidStructureIdGenerator(),
                    StructureSerializer = new SerializerForStructureBuilder()
                };

            if (idType.IsIdentity())
                return new StructureBuilder
                {
					StructureIdGenerator = identityStructureIdGenerator,
                    StructureSerializer = new SerializerForStructureBuilder()
                };

            if (idType == StructureIdTypes.String)
                return new StructureBuilderPreservingId
                {
                    StructureIdGenerator = new EmptyStructureIdGenerator(),
                    StructureSerializer = new SerializerForStructureBuilder()
                };

            throw new SisoDbException(ExceptionMessages.StructureBuilders_CreateForInsert.Inject(idType, structureSchema.Name));
        }

        public static IStructureBuilder DefaultForUpdates(IStructureSchema structureSchema)
        {
            return new StructureBuilderPreservingId
            {
                StructureIdGenerator = new EmptyStructureIdGenerator(),
                StructureSerializer = new SerializerForStructureBuilder()
            };
        }

        public static IStructureBuilder ForManualStructureIdAssignment()
        {
            return new StructureBuilderPreservingId
            {
                StructureIdGenerator = new EmptyStructureIdGenerator(),
                StructureSerializer = new SerializerForStructureBuilder()
            };
        }
    }
}