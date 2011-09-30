using SisoDb.Structures.Schemas;

namespace SisoDb.DbSchema
{
    public interface IDbSchemaSynchronizer
    {
        void Synchronize(IStructureSchema structureSchema);
    }
}