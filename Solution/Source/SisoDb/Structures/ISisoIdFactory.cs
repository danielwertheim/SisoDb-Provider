using SisoDb.Structures.Schemas;

namespace SisoDb.Structures
{
    public interface ISisoIdFactory
    {
        ISisoId GetId<T>(IStructureSchema structureSchema, T item) where T : class;
    }
}