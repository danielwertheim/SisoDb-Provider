using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.Shared.DbSchema
{
    public interface IDbSchemaSynchronizer
    {
        void Synchronize(IStructureSchema structureSchema);
    }
}