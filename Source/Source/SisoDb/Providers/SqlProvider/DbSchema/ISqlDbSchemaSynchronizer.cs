using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    public interface ISqlDbSchemaSynchronizer
    {
        void Synchronize(IStructureSchema structureSchema);
    }
}