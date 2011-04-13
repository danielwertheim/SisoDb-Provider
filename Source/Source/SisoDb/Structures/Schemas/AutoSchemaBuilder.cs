using System.Linq;
using SisoDb.Core;
using SisoDb.Cryptography;
using SisoDb.Reflections;
using SisoDb.Resources;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Structures.Schemas
{
    public class AutoSchemaBuilder : ISchemaBuilder
    {
        private readonly IHashService _hashService;

        public AutoSchemaBuilder(IHashService hashService)
        {
            _hashService = hashService.AssertNotNull("hashService");
        }

        public IStructureSchema CreateSchema(IStructureType structureType)
        {
            structureType.AssertNotNull("structureType");

            var idAccessor = GetIdAccessor(structureType);
            var indexAccessors = GetIndexAccessors(structureType);
            if (indexAccessors == null || indexAccessors.Length < 1)
                throw new SisoDbException(ExceptionMessages.AutoSchemaBuilder_MissingIndexableMembers.Inject(structureType.Name));

            var schemaName = structureType.Name;
            var schemaHash = _hashService.GenerateHash(schemaName);
            var schema = new StructureSchema(schemaName, schemaHash, idAccessor, indexAccessors);

            return schema;
        }

        private static IIdAccessor GetIdAccessor(IStructureType structureType)
        {
            if (structureType.IdProperty == null)
                throw new SisoDbException(ExceptionMessages.AutoSchemaBuilder_MissingIdMember.Inject(structureType.Name));

            if (structureType.IdProperty.PropertyType.IsGuidType() || structureType.IdProperty.PropertyType.IsNullableGuidType()
                || (structureType.IdProperty.PropertyType.IsIntType() || structureType.IdProperty.PropertyType.IsNullableIntType()))
                return new IdAccessor(structureType.IdProperty);

            throw new SisoDbException(ExceptionMessages.AutoSchemaBuilder_UnsupportedIdAccessorType.Inject(structureType.IdProperty.Name));
        }

        private static IIndexAccessor[] GetIndexAccessors(IStructureType structureType)
        {
            var indexableProperties = structureType.IndexableProperties;
            var indexAccessors = indexableProperties.Select(CreateIndexAccessor);

            return indexAccessors.ToArray();
        }

        private static IIndexAccessor CreateIndexAccessor(IStructureProperty property)
        {
            var indexAccessor = new IndexAccessor(property);

            return indexAccessor;
        }
    }
}