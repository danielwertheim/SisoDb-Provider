using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    internal interface IDbSchemaManager
    {
        void DropStructureSet(IStructureSchema structureSchema);
        void UpsertStructureSet(IStructureSchema structureSchema);
    }
}