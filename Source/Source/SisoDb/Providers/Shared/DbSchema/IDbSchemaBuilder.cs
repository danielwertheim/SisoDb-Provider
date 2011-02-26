using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.Shared.DbSchema
{
    public interface IDbSchemaBuilder
    {
        string GenerateSql(IStructureSchema structureSchema);
    }
}