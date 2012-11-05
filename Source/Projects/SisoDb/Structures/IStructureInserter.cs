using SisoDb.Structures.Schemas;

namespace SisoDb.Structures
{
    public interface IStructureInserter
    {
        void Insert(IStructureSchema structureSchema, IStructure[] structures);
        void InsertIndexesOnly(IStructureSchema structureSchema, IStructure[] structures);
        void Replace(IStructureSchema structureSchema, IStructure structure);
    }
}