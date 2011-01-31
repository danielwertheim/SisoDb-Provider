using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    internal interface ISqlDbSchemaBuilder
    {
        string GenerateSql(IStructureSchema structureSchema);
    }
}