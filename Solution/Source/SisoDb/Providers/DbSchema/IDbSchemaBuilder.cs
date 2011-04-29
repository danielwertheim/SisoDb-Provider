using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.DbSchema
{
    public interface IDbSchemaBuilder
    {
        string GenerateSql(IStructureSchema structureSchema);
    }
}