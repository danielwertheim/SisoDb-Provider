using SisoDb.PineCone.Structures.Schemas;

namespace SisoDb.PineCone.Serializers
{
    public interface IStructureSerializer
    {
        string Serialize<T>(T item, IStructureSchema structureSchema) where T : class;
    }
}