using System;
using System.Linq;
using SisoDb.Core;
using SisoDb.Reflections;
using SisoDb.Resources;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Structures.Schemas
{
    public class AutoSchemaBuilder<T> : ISchemaBuilder
        where T : class
    {
        private static readonly AutoSchemaBuilder InnerBuilder = new AutoSchemaBuilder(StructureType<T>.Instance);

        public IStructureSchema CreateSchema()
        {
            return InnerBuilder.CreateSchema();
        }
    }

    public class AutoSchemaBuilder : ISchemaBuilder
    {
        private readonly StructureType _structureType;

        public AutoSchemaBuilder(StructureType structureType)
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
            var property = _structureType.IdProperty;
            if (property == null)
                throw new SisoDbException(ExceptionMessages.AutoSchemaBuilder_MissingIdMember.Inject(_structureType.Name));

            if (property.PropertyType.IsGuidType() || property.PropertyType.IsNullableGuidType()
                || (property.PropertyType.IsIntType() || property.PropertyType.IsNullableIntType()))
                return new IdAccessor(property);

            throw new SisoDbException(ExceptionMessages.AutoSchemaBuilder_UnsupportedIdAccessorType.Inject(property.Name));
        }

        private IIndexAccessor[] GetIndexAccessors()
        {
            var indexableProperties = _structureType.IndexableProperties;
            var indexAccessors = indexableProperties.Select(CreateIndexAccessor);

            return indexAccessors.ToArray();
        }

        private static IIndexAccessor CreateIndexAccessor(IProperty property)
        {
            var indexAccessor = new IndexAccessor(property);

            return indexAccessor;
        }
    }
}