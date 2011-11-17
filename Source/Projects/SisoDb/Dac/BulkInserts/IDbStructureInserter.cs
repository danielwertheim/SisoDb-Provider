using PineCone.Structures;
using PineCone.Structures.Schemas;

namespace SisoDb.Dac.BulkInserts
{
    public interface IDbStructureInserter
    {
        void Insert(IStructureSchema structureSchema, IStructure[] structures);
    }
}