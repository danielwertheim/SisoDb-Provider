using SisoDb.Core;
using SisoDb.Cryptography;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.Builders;

namespace SisoDb.Tests.UnitTests.Structures
{
    //TODO: Remove and mock instead
    internal static class StructureSchemaTestFactory
    {
        private static readonly IStructureTypeFactory StructureTypeFactory = new StructureTypeFactory(new StructureTypeReflecter());
        private static readonly ISchemaBuilder SchemaBuilder = new AutoSchemaBuilder(new HashService());

        internal static IStructureSchema CreateSchema<T>()
            where T : class
        {
            return SchemaBuilder.CreateSchema(
                StructureTypeFactory.CreateFor(TypeFor<T>.Type));
        }
    }
}