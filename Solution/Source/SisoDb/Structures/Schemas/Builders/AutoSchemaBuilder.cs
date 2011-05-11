using System.Linq;
using SisoDb.Core;
using SisoDb.Cryptography;
using SisoDb.Reflections;
using SisoDb.Resources;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Structures.Schemas.Builders
{
    public class AutoSchemaBuilder : ISchemaBuilder
    {
        public IHashService HashService { protected get; set; }

        public AutoSchemaBuilder(IHashService hashService)
        {
            HashService = hashService.AssertNotNull("hashService");
        }

        public IStructureSchema CreateSchema(IStructureType structureType)
        {
            structureType.AssertNotNull("structureType");

            var idAccessor = GetIdAccessor(structureType);
            var indexAccessors = GetIndexAccessors(structureType);
            if (indexAccessors == null || indexAccessors.Length < 1)
                throw new SisoDbException(ExceptionMessages.AutoSchemaBuilder_MissingIndexableMembers.Inject(structureType.Name));

            var schemaHash = HashService.GenerateHash(structureType.Name);

            return new StructureSchema(
                structureType.Name, 
                schemaHash,
                idAccessor, 
                indexAccessors);
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
            return structureType.IndexableProperties
                .Select(p => new IndexAccessor(p))
                .ToArray();
        }
    }
}