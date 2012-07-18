using PineCone.Structures.Schemas;

namespace PineCone.Structures.IdGenerators
{
    public class EmptyStructureIdGenerator : IStructureIdGenerator
    {
        public IStructureId Generate(IStructureSchema structureSchema)
        {
            return null;
        }

        public IStructureId[] Generate(IStructureSchema structureSchema, int numOfIds)
        {
            return null;
        }
    }
}