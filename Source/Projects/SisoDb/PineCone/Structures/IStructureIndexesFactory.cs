using SisoDb.PineCone.Structures.Schemas;

namespace SisoDb.PineCone.Structures
{
    public interface IStructureIndexesFactory
    {
        IStructureIndex[] CreateIndexes<T>(IStructureSchema structureSchema, T item, IStructureId structureId) where T : class;
    }
}