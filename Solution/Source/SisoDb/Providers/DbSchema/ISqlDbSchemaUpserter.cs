using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.DbSchema
{
    public interface ISqlDbSchemaUpserter
    {
        void Upsert(IStructureSchema structureSchema);
    }
}