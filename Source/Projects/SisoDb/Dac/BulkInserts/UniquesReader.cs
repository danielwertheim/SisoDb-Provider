using System;
using System.Collections.Generic;
using PineCone.Structures;
using SisoDb.DbSchema;

namespace SisoDb.Dac.BulkInserts
{
    public class UniquesReader : SingleResultReaderBase<IStructureIndex>
    {
        public UniquesReader(UniqueStorageSchema storageSchema, IEnumerable<IStructureIndex> items)
            : base(storageSchema, items)
        {
        }

        public override object GetValue(int ordinal)
        {
            if (ordinal == UniqueStorageSchema.Fields.RowId.Ordinal)
                return DBNull.Value;

            if (ordinal == UniqueStorageSchema.Fields.StructureId.Ordinal)
                return Enumerator.Current.StructureId.Value;

            if (ordinal == UniqueStorageSchema.Fields.UqStructureId.Ordinal)
            {
                if (Enumerator.Current.IndexType == StructureIndexType.UniquePerType)
                    return DBNull.Value;
                
                return Enumerator.Current.StructureId.Value;
            }

            if (ordinal == UniqueStorageSchema.Fields.UqMemberPath.Ordinal)
                return Enumerator.Current.Path;

            if (ordinal == UniqueStorageSchema.Fields.UqValue.Ordinal)
                return SisoEnvironment.HashService.GenerateHash(SisoEnvironment.StringConverter.AsString(Enumerator.Current.Value));

            throw new NotSupportedException();
        }
    }
}