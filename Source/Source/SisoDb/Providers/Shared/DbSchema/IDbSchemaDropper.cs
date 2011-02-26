using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.Shared.DbSchema
{
    public interface IDbSchemaDropper
    {
        void Drop(IStructureSchema structureSchema);
    }
}