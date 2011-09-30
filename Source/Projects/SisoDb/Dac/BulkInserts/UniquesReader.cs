using System;
using System.Collections.Generic;
using SisoDb.DbSchema;
using SisoDb.Structures;

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
            var schemaField = StorageSchema[ordinal];

            if (schemaField.Name == UniqueStorageSchema.Fields.SisoId.Name)
                return Enumerator.Current.SisoId.Value;

            if (schemaField.Name == UniqueStorageSchema.Fields.UqSisoId.Name)
            {
                if (Enumerator.Current.IndexType == StructureIndexType.UniquePerType)
                    return DBNull.Value;
                
                return Enumerator.Current.SisoId.Value;
            }

            if (schemaField.Name == UniqueStorageSchema.Fields.UqName.Name)
                return Enumerator.Current.Name;

            if (schemaField.Name == UniqueStorageSchema.Fields.UqValue.Name)
                return SisoEnvironment.Formatting.StringConverter.AsString(Enumerator.Current.Value);

            throw new NotSupportedException();
        }
    }
}