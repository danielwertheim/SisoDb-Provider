using SisoDb.Serialization;
using SisoDb.Structures.Schemas;

namespace SisoDb.Structures
{
    public interface IStructureBuilder
    {
        IJsonSerializer JsonSerializer { get; }

        IStructureIdFactory IdFactory { get; }
        
        IStructureIndexesFactory IndexesFactory { get; }

        IStructure CreateStructure<T>(T item, IStructureSchema structureSchema) where T : class;
    }
}