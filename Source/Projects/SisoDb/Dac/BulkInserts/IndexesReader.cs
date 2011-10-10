using System;
using System.Collections.Generic;
using NCore.Reflections;
using PineCone.Structures;
using SisoDb.DbSchema;

namespace SisoDb.Dac.BulkInserts
{
    public class IndexesReader : SingleResultReaderBase<IStructureIndex>
    {
        public IndexesReader(IndexStorageSchema storageSchema, IEnumerable<IStructureIndex> items)
            : base(storageSchema, items)
        {
        }

        public override object GetValue(int ordinal)
        {
            //TODO: Extract to NCore and cache.!!!!
            if (ordinal == 0)
                return Enumerator.Current.StructureId.Value;

            var schemaField = StorageSchema[ordinal];
            
            if (schemaField.Name == IndexStorageSchema.Fields.MemberPath.Name)
                return Enumerator.Current.Path;

            var dataType = Enumerator.Current.Value.GetType();

            if (schemaField.Name == IndexStorageSchema.Fields.StringValue.Name && (dataType.IsStringType() || dataType.IsEnumType() || dataType.IsNullableEnumType() ))
                return Enumerator.Current.Value;

            if (schemaField.Name == IndexStorageSchema.Fields.IntegerValue.Name && (dataType.IsIntType() || dataType.IsNullableIntType()))
                return Enumerator.Current.Value;

            if (schemaField.Name == IndexStorageSchema.Fields.IntegerValue.Name && (dataType.IsLongType() || dataType.IsNullableLongType()))
                return Enumerator.Current.Value;

            if (schemaField.Name == IndexStorageSchema.Fields.IntegerValue.Name && (dataType.IsShortType() || dataType.IsNullableShortType()))
                return Enumerator.Current.Value;

            if (schemaField.Name == IndexStorageSchema.Fields.FractalValue.Name && (dataType.IsSingleType() || dataType.IsNullableSingleType()))
                return Enumerator.Current.Value;

            if (schemaField.Name == IndexStorageSchema.Fields.FractalValue.Name && (dataType.IsFloatType() || dataType.IsNullableFloatType()))
                return Enumerator.Current.Value;

            if (schemaField.Name == IndexStorageSchema.Fields.FractalValue.Name && (dataType.IsDecimalType() || dataType.IsNullableDecimalType()))
                return Enumerator.Current.Value;

            if (schemaField.Name == IndexStorageSchema.Fields.FractalValue.Name && (dataType.IsFloatType() || dataType.IsNullableFloatType()))
                return Enumerator.Current.Value;

            if (schemaField.Name == IndexStorageSchema.Fields.DateTimeValue.Name && (dataType.IsDateTimeType() || dataType.IsNullableDateTimeType()))
                return Enumerator.Current.Value;

            if (schemaField.Name == IndexStorageSchema.Fields.BoolValue.Name && (dataType.IsBoolType() || dataType.IsNullableBoolType()))
                return Enumerator.Current.Value;

            return DBNull.Value;
        }
    }
}