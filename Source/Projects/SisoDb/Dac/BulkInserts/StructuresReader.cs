using System;
using System.Collections.Generic;
using SisoDb.DbSchema;
using SisoDb.Structures;

namespace SisoDb.Dac.BulkInserts
{
    public class StructuresReader : SingleResultReaderBase<IStructure>
    {
        public StructuresReader(StructureStorageSchema storageSchema, IEnumerable<IStructure> items)
            : base(storageSchema, items)
        {
        }

        public override object GetValue(int ordinal)
        {
            if (ordinal == StructureStorageSchema.Fields.RowId.Ordinal)
                return DBNull.Value;

            if (ordinal == StructureStorageSchema.Fields.Id.Ordinal)
                return Enumerator.Current.Id.Value;

            if(ordinal == StructureStorageSchema.Fields.Json.Ordinal)
                return Enumerator.Current.Data;

            throw new NotSupportedException();
        }
    }
}