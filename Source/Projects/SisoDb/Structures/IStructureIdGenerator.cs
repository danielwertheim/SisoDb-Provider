using SisoDb.Structures.Schemas;

namespace SisoDb.Structures
{
    public interface IStructureIdGenerator
    {
        IStructureId Generate(IStructureSchema structureSchema);
        IStructureId[] Generate(IStructureSchema structureSchema, int numOfIds);
    }
}