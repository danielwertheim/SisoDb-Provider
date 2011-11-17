using PineCone.Structures.Schemas;
using SisoDb.Dac;

namespace SisoDb.DbSchema
{
    public interface IDbSchemaManager
    {
        void ClearCache();
        void DropStructureSet(IStructureSchema structureSchema, IDbClient dbClient);
        void UpsertStructureSet(IStructureSchema structureSchema, IDbSchemaUpserter upserter);
    }
}