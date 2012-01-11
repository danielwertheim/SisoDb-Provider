using System.Collections.Generic;
using PineCone.Structures;
using SisoDb.DbSchema;

namespace SisoDb.Dac.BulkInserts
{

    public abstract class IndexesReader : SingleResultReaderBase<IStructureIndex>
    {
    	protected IndexesReader(IndexStorageSchema storageSchema, IEnumerable<IStructureIndex> items)
            : base(storageSchema, items)
        {
        }
    }
}