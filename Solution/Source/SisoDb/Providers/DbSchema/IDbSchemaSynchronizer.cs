using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.DbSchema
{
    public interface IDbSchemaSynchronizer
    {
        void Synchronize(IStructureSchema structureSchema);
    }
}