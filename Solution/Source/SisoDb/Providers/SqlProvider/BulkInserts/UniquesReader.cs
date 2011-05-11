using System;
using System.Collections.Generic;
using SisoDb.Providers.SqlProvider.DbSchema;
using SisoDb.Structures;

namespace SisoDb.Providers.SqlProvider.BulkInserts
{
    public class UniquesReader : SingleResultReaderBase<IStructureIndex>
    {
        public UniquesReader(UniqueStorageSchema storageSchema, IEnumerable<IStructureIndex> items)
            : base(storageSchema, items)
        {
        }

        public override object GetValue(int ordinal)
        {
            var schemaField = StorageSchema.FieldsByIndex[ordinal];

            if (schemaField.Name == UniqueStorageSchema.Fields.SisoId.Name)
                return Enumerator.Current.SisoId.Value;

            if (schemaField.Name == UniqueStorageSchema.Fields.UqSisoId.Name)
            {
                if (Enumerator.Current.IndexType == StructureIndexType.UniquePerType)
                    return DBNull.Value;

                if (Enumerator.Current.SisoId != null && Enumerator.Current.SisoId.Value != null)
                    return Enumerator.Current.SisoId.Value;

                return DBNull.Value;
            }

            if (schemaField.Name == UniqueStorageSchema.Fields.UqName.Name)
                return Enumerator.Current.Name;

            if (schemaField.Name == UniqueStorageSchema.Fields.UqValue.Name)
                return SisoEnvironment.Formatting.StringConverter.AsString(Enumerator.Current.Value);
            
            throw new NotSupportedException();
        }
    }
}