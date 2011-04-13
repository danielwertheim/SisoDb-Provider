using SisoDb.Cryptography;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.UnitTests.Structures
{
    internal static class StructureSchemaTestFactory
    {
        internal static IStructureSchema CreateSchema<T>()
            where T : class
        {
            return new AutoSchemaBuilder(new HashService()).CreateSchema(StructureTypeFor<T>.Instance);
        }
    }
}