using System;
using EnsureThat;
using NCore;
using PineCone.Structures;
using PineCone.Structures.IdGenerators;
using PineCone.Structures.Schemas;
using SisoDb.Resources;
using SisoDb.Serialization;

namespace SisoDb.Structures
{
    public class StructureBuilders : IStructureBuilders
    {
    	public Func<IStructureSchema, IIdentityStructureIdGenerator, IStructureBuilder> ForInserts { get; set; }

        public Func<IStructureSchema, IStructureBuilder> ForUpdates { get; set; }

		public StructureBuilders()
		{
			ForInserts = CreateForInserts;
            ForUpdates = CreateForUpdates;
        }

		private IStructureBuilder CreateForInserts(IStructureSchema structureSchema, IIdentityStructureIdGenerator identityStructureIdGenerator)
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

        private static IStructureBuilder CreateForUpdates(IStructureSchema structureSchema)
        {
            return new StructureBuilderPreservingId
            {
                StructureIdGenerator = new EmptyStructureIdGenerator(),
                StructureSerializer = new SerializerForStructureBuilder()
            };
        }
    }
}