using SisoDb.Structures;

namespace SisoDb.Providers.SqlProvider.BulkInserts
{
    public class IndexRow
    {
        public ISisoId SisoId { get; private set; }

        public IStructureIndex[] Indexes { get; private set; }

        public IndexRow(ISisoId sisoId, IStructureIndex[] indexes)
        {
            SisoId = sisoId;
            Indexes = indexes;
        }

        public object GetValue(int index)
        {
            return index == 0 ? SisoId.Value : Indexes[index-1].Value;
        }
    }
}