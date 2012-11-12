using SisoDb.Dac;
using SisoDb.EnsureThat;
using SisoDb.NCore;
using SisoDb.Resources;
using SisoDb.Structures.IdGenerators;
using SisoDb.Structures.Schemas;

namespace SisoDb.Structures
{
    public static class StructureBuildersFn
    {
        public static IStructureBuilder GetDefaultBuilderForInserts(IStructureBuilders builders, IStructureSchema schema, IDbClient dbClient)
        {
            Ensure.That(builders, "builders").IsNotNull();
            Ensure.That(schema, "schema").IsNotNull();
            Ensure.That(dbClient, "dbClient").IsNotNull();

            var idType = schema.IdAccessor.IdType;
            if (idType.IsGuid())
                return new StructureBuilder
                {
                    StructureIdGenerator = builders.GuidStructureIdGeneratorFn(schema),
                    StructureSerializer = builders.StructureSerializerFn()
                };

            if (idType.IsIdentity())
                return new StructureBuilder
                {
                    StructureIdGenerator = builders.IdentityStructureIdGeneratorFn(schema, dbClient),
                    StructureSerializer = builders.StructureSerializerFn()
                };

            if (idType.IsString())
                return new StructureBuilderPreservingId
                {
                    StructureIdGenerator = new EmptyStructureIdGenerator(),
                    StructureSerializer = builders.StructureSerializerFn()
                };

            throw new SisoDbException(ExceptionMessages.StructureBuilders_CreateForInsert.Inject(idType, schema.Name));
        }

        public static IStructureBuilder GetBuilderForInsertsPreservingId(IStructureBuilders builders, IStructureSchema schema, IDbClient dbClient)
        {
            Ensure.That(builders, "builders").IsNotNull();
            Ensure.That(schema, "schema").IsNotNull();
            Ensure.That(dbClient, "dbClient").IsNotNull();

            IStructureIdGenerator idGenerator;
            var idType = schema.IdAccessor.IdType;

            if (idType.IsGuid())
                idGenerator = new EmptyStructureIdGenerator();
            else if (idType.IsIdentity())
                idGenerator = builders.IdentityStructureIdGeneratorFn(schema, dbClient);
            else if (idType.IsString())
                idGenerator = new EmptyStructureIdGenerator();
            else
                throw new SisoDbException(ExceptionMessages.StructureBuilders_CreateForInsert.Inject(idType, schema.Name));

            return new StructureBuilderPreservingId
            {
                StructureIdGenerator = idGenerator,
                StructureSerializer = builders.StructureSerializerFn()
            };
        }

        public static IStructureBuilder GetBuilderForInsertsAssigningIfMissingId(IStructureBuilders builders, IStructureSchema schema, IDbClient dbClient)
        {
            Ensure.That(builders, "builders").IsNotNull();
            Ensure.That(schema, "schema").IsNotNull();
            Ensure.That(dbClient, "dbClient").IsNotNull();

            IStructureIdGenerator idGenerator;
            var idType = schema.IdAccessor.IdType;

            if (idType.IsGuid())
                idGenerator = builders.GuidStructureIdGeneratorFn(schema);
            else if (idType.IsIdentity())
                idGenerator = builders.IdentityStructureIdGeneratorFn(schema, dbClient);
            else if (idType.IsString())
                idGenerator = new EmptyStructureIdGenerator();
            else
                throw new SisoDbException(ExceptionMessages.StructureBuilders_CreateForInsert.Inject(idType, schema.Name));

            return new StructureBuilderAutoId
            {
                StructureIdGenerator = idGenerator,
                StructureSerializer = builders.StructureSerializerFn()
            };
        }

        public static IStructureBuilder GetBuilderForManualIdAssignment(IStructureBuilders builders, IStructureSchema schema, IDbClient dbClient)
        {
            return new StructureBuilderPreservingId
            {
                StructureIdGenerator = new EmptyStructureIdGenerator(),
                StructureSerializer = builders.StructureSerializerFn()
            };
        }
    }
}