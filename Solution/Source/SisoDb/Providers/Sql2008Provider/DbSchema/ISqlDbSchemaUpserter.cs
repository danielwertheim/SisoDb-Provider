using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.Sql2008Provider.DbSchema
{
    public interface ISqlDbSchemaUpserter
    {
        void Upsert(IStructureSchema structureSchema);
    }
}