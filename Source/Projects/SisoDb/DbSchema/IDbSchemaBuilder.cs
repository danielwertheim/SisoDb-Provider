using SisoDb.Structures.Schemas;

namespace SisoDb.DbSchema
{
    public interface IDbSchemaBuilder
    {
        string GenerateSql(IStructureSchema structureSchema);
    }
}