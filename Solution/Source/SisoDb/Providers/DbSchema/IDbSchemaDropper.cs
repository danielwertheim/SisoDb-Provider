using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.DbSchema
{
    public interface IDbSchemaDropper
    {
        void Drop(IStructureSchema structureSchema);
    }
}