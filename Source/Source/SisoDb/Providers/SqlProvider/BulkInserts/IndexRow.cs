using SisoDb.Structures;

namespace SisoDb.Providers.SqlProvider.BulkInserts
{
    internal class IndexRow
    {
        internal IStructureId StructureId { get; private set; }

        internal IStructureIndex[] Indexes { get; private set; }

        internal IndexRow(IStructureId structureId, IStructureIndex[] indexes)
        {
            StructureId = structureId;
            Indexes = indexes;
        }

        internal object GetValue(int index)
        {
            return index == 0 ? StructureId.Value : Indexes[index-1].Value;
        }
    }
}