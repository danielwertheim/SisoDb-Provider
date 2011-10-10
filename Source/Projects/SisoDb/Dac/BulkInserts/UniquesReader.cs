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
            var schemaField = StorageSchema[ordinal];

            if (schemaField.Name == UniqueStorageSchema.Fields.StructureId.Name)
                return Enumerator.Current.StructureId.Value;

            if (schemaField.Name == UniqueStorageSchema.Fields.UqStructureId.Name)
            {
                if (Enumerator.Current.IndexType == StructureIndexType.UniquePerType)
                    return DBNull.Value;
                
                return Enumerator.Current.StructureId.Value;
            }

            if (schemaField.Name == UniqueStorageSchema.Fields.UqMemberPath.Name)
                return Enumerator.Current.Path;

            if (schemaField.Name == UniqueStorageSchema.Fields.UqValue.Name)
                return SisoEnvironment.StringConverter.AsString(Enumerator.Current.Value);

            throw new NotSupportedException();
        }
    }
}