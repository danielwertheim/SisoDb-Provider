using SisoDb.Structures;

namespace SisoDb.Providers.SqlProvider.BulkInserts
{
    public class IndexRow
    {
        public IStructureId StructureId { get; private set; }

        public IStructureIndex[] Indexes { get; private set; }

        public IndexRow(IStructureId structureId, IStructureIndex[] indexes)
        {
            StructureId = structureId;
            Indexes = indexes;
        }

        public object GetValue(int index)
        {
            return index == 0 ? StructureId.Value : Indexes[index-1].Value;
        }
    }
}