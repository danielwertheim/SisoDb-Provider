using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider
{
    internal static class StructureSchemaExtensions
    {
        internal static string GetStructureTableName(this IStructureSchema structureSchema)
        {
            return structureSchema.Name + "Structure";
        }

        internal static string GetIndexesTableName(this IStructureSchema structureSchema)
        {
            return structureSchema.Name + "Indexes";
        }

        internal static string GetUniquesTableName(this IStructureSchema structureSchema)
        {
            return structureSchema.Name + "Uniques";
        }
    }
}