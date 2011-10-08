using System.Collections.Generic;
using PineCone.Structures;
using PineCone.Structures.Schemas;

namespace SisoDb.Dac.BulkInserts
{
    public interface IDbBulkInserter
    {
        void Insert(IStructureSchema structureSchema, IEnumerable<IStructure> structures);
    }
}