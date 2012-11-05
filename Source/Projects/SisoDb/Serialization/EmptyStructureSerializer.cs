using SisoDb.Structures.Schemas;

namespace SisoDb.Serialization
{
    public class EmptyStructureSerializer : IStructureSerializer
    {
        public string Serialize<T>(T structure, IStructureSchema structureSchema) where T : class
        {
            return null;
        }
    }
}