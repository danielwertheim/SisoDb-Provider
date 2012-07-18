using SisoDb.PineCone.Structures.Schemas;

namespace SisoDb.PineCone.Structures
{
    public interface IStructureIdGenerator
    {
        IStructureId Generate(IStructureSchema structureSchema);

        IStructureId[] Generate(IStructureSchema structureSchema, int numOfIds);
    }
}