using SisoDb.PineCone.Structures.Schemas;

namespace SisoDb.PineCone.Structures.IdGenerators
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