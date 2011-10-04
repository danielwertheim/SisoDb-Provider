using PineCone.Structures.Schemas;

namespace SisoDb.DbSchema
{
    public interface IDbSchemaUpserter
    {
        void Upsert(IStructureSchema structureSchema);
    }
}