using System.Linq;
using SisoDb.Reflections;
using SisoDb.Resources;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Structures.Schemas
{
    public class AutoSchemaBuilder<T> : ISchemaBuilder
        where T : class
    {
        private static readonly StructureTypeInfo TypeInfoState;

        static AutoSchemaBuilder()
        {
            TypeInfoState = new StructureTypeInfo(typeof(T));
        }

        public IStructureSchema CreateSchema()
        {
            var idAccessor = GetIdAccessor();
            var indexAccessors = GetIndexAccessors();
            if (indexAccessors == null || indexAccessors.Length < 1)
                throw new SisoDbException(ExceptionMessages.AutoSchemaBuilder_MissingIndexableMembers.Inject(TypeInfoState.Name));

            var schemaName = TypeInfoState.Name;
            var schemaHash = SisoDbEnvironment.HashService.GenerateHash(schemaName);
            var schema = new StructureSchema(schemaName, schemaHash, idAccessor, indexAccessors);

            return schema;
        }

        private static IIdAccessor GetIdAccessor()
        {
            var property = TypeInfoState.IdProperty;
            if (property == null)
                throw new SisoDbException(ExceptionMessages.AutoSchemaBuilder_MissingIdMember.Inject(TypeInfoState.Name));

            if (property.PropertyType.IsGuidType() || property.PropertyType.IsNullableGuidType()
                || (property.PropertyType.IsIntType() || property.PropertyType.IsNullableIntType()))
                return new IdAccessor(property);

            throw new SisoDbException(ExceptionMessages.AutoSchemaBuilder_UnsupportedIdAccessorType.Inject(property.Name));
        }

        private static IIndexAccessor[] GetIndexAccessors()
        {
            var indexableProperties = TypeInfoState.IndexableProperties;
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