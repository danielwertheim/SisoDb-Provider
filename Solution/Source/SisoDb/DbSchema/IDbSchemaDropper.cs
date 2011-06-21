using SisoDb.Structures.Schemas;

namespace SisoDb.DbSchema
{
    public interface IDbSchemaDropper
    {
        void Drop(IStructureSchema structureSchema);
    }
}