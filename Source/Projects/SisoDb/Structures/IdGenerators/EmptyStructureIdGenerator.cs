using SisoDb.Structures.Schemas;

namespace SisoDb.Structures.IdGenerators
{
    public class EmptyStructureIdGenerator : IStructureIdGenerator
    {
        public virtual IStructureId Generate(IStructureSchema structureSchema)
        {
            return null;
        }

        public virtual IStructureId[] Generate(IStructureSchema structureSchema, int numOfIds)
        {
            return null;
        }
    }
}