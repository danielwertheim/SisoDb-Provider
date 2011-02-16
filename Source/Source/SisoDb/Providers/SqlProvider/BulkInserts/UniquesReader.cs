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
            if (schemaField.Name == UniqueStorageSchema.Fields.StructureId.Name)
            {
                var structureId = Enumerator.Current.StructureId;
                if (structureId != null && structureId.Value != null)
                    return structureId.Value;

                return DBNull.Value;
            }

            if (schemaField.Name == UniqueStorageSchema.Fields.Name.Name)
                return Enumerator.Current.Name;

            if (schemaField.Name == UniqueStorageSchema.Fields.Value.Name)
                return SisoDbEnvironment.Formatting.StringConverter.AsString(Enumerator.Current.Value); //return Enumerator.Current.Value;

            throw new NotSupportedException();
        }
    }
}