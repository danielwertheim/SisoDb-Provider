using PineCone.Structures.Schemas;

namespace SisoDb.DbSchema
{
    public interface IDbSchemaManager
    {
        void DropStructureSet(IStructureSchema structureSchema, IDbSchemaDropper dropper);
        void UpsertStructureSet(IStructureSchema structureSchema, IDbSchemaUpserter upserter);
    }
}