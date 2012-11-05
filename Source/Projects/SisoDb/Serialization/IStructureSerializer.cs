using SisoDb.Structures.Schemas;

namespace SisoDb.Serialization
{
    public interface IStructureSerializer
    {
        string Serialize<T>(T structure, IStructureSchema structureSchema) where T : class;
    }
}