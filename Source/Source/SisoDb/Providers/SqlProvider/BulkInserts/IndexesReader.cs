using System.Collections.Generic;
using SisoDb.Providers.SqlProvider.DbSchema;

namespace SisoDb.Providers.SqlProvider.BulkInserts
{
    internal class IndexesReader : SingleResultReaderBase<IndexRow>
    {
        internal IndexesReader(IndexStorageSchema storageSchema, IEnumerable<IndexRow> items)
            : base(storageSchema, items)
        {
        }

        public override object GetValue(int ordinal)
        {
            return Enumerator.Current.GetValue(ordinal);
        }
    }
}