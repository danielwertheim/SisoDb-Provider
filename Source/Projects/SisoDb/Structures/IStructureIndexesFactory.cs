using SisoDb.Structures.Schemas;

namespace SisoDb.Structures
{
    public interface IStructureIndexesFactory
    {
        IStructureIndex[] CreateIndexes<T>(IStructureSchema structureSchema, T item, IStructureId structureId) where T : class;
    }
}