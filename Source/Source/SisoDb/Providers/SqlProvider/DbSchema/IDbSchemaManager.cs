using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    public interface IDbSchemaManager
    {
        void DropStructureSet(IStructureSchema structureSchema);
        void UpsertStructureSet(IStructureSchema structureSchema);
    }
}