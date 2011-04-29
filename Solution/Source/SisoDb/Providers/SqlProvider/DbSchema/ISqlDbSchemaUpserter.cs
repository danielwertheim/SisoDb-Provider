using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    public interface ISqlDbSchemaUpserter
    {
        void Upsert(IStructureSchema structureSchema);
    }
}