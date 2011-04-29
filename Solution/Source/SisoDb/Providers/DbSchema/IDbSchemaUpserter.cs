using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.DbSchema
{
    public interface IDbSchemaUpserter
    {
        void Upsert(IStructureSchema structureSchema);
    }
}