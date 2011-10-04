using System;
using System.Collections.Generic;
using PineCone.Structures;
using SisoDb.DbSchema;

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
            var schemaField = StorageSchema[ordinal];
            if(schemaField.Name == StructureStorageSchema.Fields.Id.Name)
            {
                if (Enumerator.Current.Id.IdType == IdTypes.Identity)
                    return Enumerator.Current.Id.Value;

                if (Enumerator.Current.Id.IdType == IdTypes.Guid)
                    return Enumerator.Current.Id.Value;

                throw new NotSupportedException();
            }

            if(schemaField.Name == StructureStorageSchema.Fields.Json.Name)
                return Enumerator.Current.Json;

            throw new NotSupportedException();
        }
    }
}