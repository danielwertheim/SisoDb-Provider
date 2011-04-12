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

            if (schemaField.Name == UniqueStorageSchema.Fields.SisoIdRef.Name)
                return Enumerator.Current.SisoId.Value;

            if (schemaField.Name == UniqueStorageSchema.Fields.SisoId.Name)
            {
                if (Enumerator.Current.Uniqueness == StructureIndexUniques.PerType)
                    return DBNull.Value;

                var sisoId = Enumerator.Current.SisoId;
                if (sisoId != null && sisoId.Value != null)
                    return sisoId.Value;

                return DBNull.Value;
            }

            if (schemaField.Name == UniqueStorageSchema.Fields.Name.Name)
                return Enumerator.Current.Name;

            if (schemaField.Name == UniqueStorageSchema.Fields.Value.Name)
                return SisoDbEnvironment.Formatting.StringConverter.AsString(Enumerator.Current.Value);
            
            throw new NotSupportedException();
        }
    }
}