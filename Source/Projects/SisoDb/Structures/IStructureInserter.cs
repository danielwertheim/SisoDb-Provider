using PineCone.Structures;
using PineCone.Structures.Schemas;

namespace SisoDb.Structures
{
    public interface IStructureInserter
    {
        void Insert(IStructureSchema structureSchema, IStructure[] structures);
        void Replace(IStructureSchema structureSchema, IStructure structure);
    }
}