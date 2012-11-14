using SisoDb.DbSchema;
using SisoDb.Structures.Schemas;

namespace SisoDb.Spatial.DbSchema
{
    public static class SpatialDbSchemaInfo
    {
        public static class Suffixes
        {
            public const string SpatialTableNameSuffix = "Spatial";
        }

        public static string GetSpatialTableName(this IStructureSchema structureSchema)
        {
            return GenerateSpatialTableName(structureSchema.Name);
        }

        public static string GenerateSpatialTableName(string structureName)
        {
            return string.Concat(DbSchemaNamingPolicy.GenerateFor(structureName), Suffixes.SpatialTableNameSuffix);
        }
    }
}