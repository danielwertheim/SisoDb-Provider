using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    internal interface ISqlDbSchemaSynchronizer
    {
        void Synchronize(IStructureSchema structureSchema);
    }
}