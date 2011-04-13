using System.Linq;
using SisoDb.Core;
using SisoDb.Reflections;
using SisoDb.Resources;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Structures.Schemas
{
    public class AutoSchemaBuilder : ISchemaBuilder
    {
        private readonly IStructureType _structureType;

        public AutoSchemaBuilder(IStructureType structureType)
        {
            _structureType = structureType.AssertNotNull("structureType");
        }

        public IStructureSchema CreateSchema()
        {
            var idAccessor = GetIdAccessor();
            var indexAccessors = GetIndexAccessors();
            if (indexAccessors == null || indexAccessors.Length < 1)
                throw new SisoDbException(ExceptionMessages.AutoSchemaBuilder_MissingIndexableMembers.Inject(_structureType.Name));

            var schemaName = _structureType.Name;
            var schemaHash = SisoDbEnvironment.HashService.GenerateHash(schemaName);
            var schema = new StructureSchema(schemaName, schemaHash, idAccessor, indexAccessors);

            return schema;
        }

        private IIdAccessor GetIdAccessor()
        {
            if (_structureType.IdProperty == null)
                throw new SisoDbException(ExceptionMessages.AutoSchemaBuilder_MissingIdMember.Inject(_structureType.Name));

            if (_structureType.IdProperty.PropertyType.IsGuidType() || _structureType.IdProperty.PropertyType.IsNullableGuidType()
                || (_structureType.IdProperty.PropertyType.IsIntType() || _structureType.IdProperty.PropertyType.IsNullableIntType()))
                return new IdAccessor(_structureType.IdProperty);

            throw new SisoDbException(ExceptionMessages.AutoSchemaBuilder_UnsupportedIdAccessorType.Inject(_structureType.IdProperty.Name));
        }

        private IIndexAccessor[] GetIndexAccessors()
        {
            var indexableProperties = _structureType.IndexableProperties;
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