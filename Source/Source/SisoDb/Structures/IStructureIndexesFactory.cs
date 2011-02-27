using System.Collections.Generic;
using SisoDb.Structures.Schemas;

namespace SisoDb.Structures
{
    public interface IStructureIndexesFactory
    {
        IEnumerable<IStructureIndex> GetIndexes<T>(IStructureSchema structureSchema, T item, IStructureId id)
            where T : class;
    }
}