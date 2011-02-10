using System.Linq;
using SisoDb.Resources;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Structures.Schemas
{
    internal class AutoSchemaBuilder<T> : ISchemaBuilder<T>
        where T : class
    {
        private const string IdName = "Id";
        private static readonly TypeInfo TypeInfoState;
        private static readonly string[] NonIndexableNames;

        static AutoSchemaBuilder()
        {
            TypeInfoState = new TypeInfo(typeof(T));
            NonIndexableNames = new[] { IdName };
        }

        public IStructureSchema CreateSchema()
        {
            var idAccessor = GetIdAccessor();
            if (idAccessor == null)
                throw new SisoDbException(ExceptionMessages.AutoSchemaBuilder_MissingIdMember.Inject(TypeInfoState.Name));

            var indexAccessors = GetIndexAccessors();
            if (indexAccessors == null || indexAccessors.Length < 1)
                throw new SisoDbException(ExceptionMessages.AutoSchemaBuilder_MissingIndexableMembers.Inject(TypeInfoState.Name));

            var schemaName = TypeInfoState.Name;
            var schema = new StructureSchema(schemaName, idAccessor, indexAccessors);

            return schema;
        }

        private static IIdAccessor GetIdAccessor()
        {
            var property = TypeInfoState.GetIdProperty(IdName);
            if (property == null)
                return null;

            if (property.PropertyType.IsGuidType() || property.PropertyType.IsNullableGuidType() 
                || (property.PropertyType.IsIntType() || property.PropertyType.IsNullableIntType()))
                return new IdAccessor(property);
            
            throw new SisoDbException(ExceptionMessages.AutoSchemaBuilder_UnsupportedIdAccessorType.Inject(property.Name));
        }

        private static IIndexAccessor[] GetIndexAccessors()
        {
            var indexableProperties = TypeInfoState.GetIndexableProperties(NonIndexableNames);
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