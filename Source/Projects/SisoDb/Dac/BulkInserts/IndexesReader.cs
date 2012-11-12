using System.Collections.Generic;
using SisoDb.DbSchema;
using SisoDb.Structures;

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