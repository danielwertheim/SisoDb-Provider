using System.Collections.Generic;
using SisoDb.PineCone.Structures;
using SisoDb.PineCone.Structures.Schemas;

namespace SisoDb.PineCone
{
    public interface IPineConizer
    {
        IStructureSchemas Schemas { get; set; }
        IStructureBuilder Builder { get; set; }
        IStructure CreateStructureFor<T>(T item) where T : class;
        IEnumerable<IStructure> CreateStructuresFor<T>(T[] items) where T : class;
    }
}