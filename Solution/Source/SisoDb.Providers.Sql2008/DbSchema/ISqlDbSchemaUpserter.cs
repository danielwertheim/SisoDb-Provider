using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.Sql2008.DbSchema
{
    public interface ISqlDbSchemaUpserter
    {
        void Upsert(IStructureSchema structureSchema);
    }
}