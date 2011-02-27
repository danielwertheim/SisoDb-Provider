using SisoDb.Structures.Schemas;

namespace SisoDb.Structures
{
    public interface IStructureIdFactory
    {
        IStructureId GetId<T>(IStructureSchema structureSchema, T item) where T : class;
    }
}