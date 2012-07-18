using PineCone.Structures.Schemas;

namespace PineCone.Structures
{
    public interface IStructureIdGenerator
    {
        IStructureId Generate(IStructureSchema structureSchema);

        IStructureId[] Generate(IStructureSchema structureSchema, int numOfIds);
    }
}