using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    public interface ISqlDbSchemaBuilder
    {
        string GenerateSql(IStructureSchema structureSchema);
    }
}