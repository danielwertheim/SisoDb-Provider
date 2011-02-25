using SisoDb.Structures.Schemas;

namespace SisoDb.Structures
{
    public interface IStructureBuilder
    {
        IStructure CreateStructure<T>(T item, IStructureSchema structureSchema) where T : class;
    }
}