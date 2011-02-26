using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.Shared.DbSchema
{
    public interface IDbSchemaUpserter
    {
        void Upsert(IStructureSchema structureSchema);
    }
}