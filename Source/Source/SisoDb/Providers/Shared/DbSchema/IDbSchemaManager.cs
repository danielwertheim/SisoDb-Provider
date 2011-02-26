using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.Shared.DbSchema
{
    public interface IDbSchemaManager
    {
        void DropStructureSet(IStructureSchema structureSchema, IDbSchemaDropper dropper);
        void UpsertStructureSet(IStructureSchema structureSchema, IDbSchemaUpserter upserter);
    }
}