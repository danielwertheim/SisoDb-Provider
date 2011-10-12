using System;
using NCore;
using NCore.Reflections;
using PineCone.Structures.Schemas;
using SisoDb.Providers;
using SisoDb.Resources;

namespace SisoDb.DbSchema
{
    [Serializable]
    public class IndexStorageSchema : StorageSchemaBase
    {
        public static class Fields
        {
            public static readonly SchemaField StructureId = new SchemaField(0, StructureSchema.IdMemberName);
            public static readonly SchemaField MemberPath = new SchemaField(1, "MemberPath");
            public static readonly SchemaField StringValue = new SchemaField(2, "StringValue");
            public static readonly SchemaField IntegerValue = new SchemaField(3, "IntegerValue");
            public static readonly SchemaField FractalValue = new SchemaField(4, "FractalValue");
            public static readonly SchemaField DateTimeValue = new SchemaField(5, "DateTimeValue");
            public static readonly SchemaField BoolValue = new SchemaField(6, "BoolValue");
            public static readonly SchemaField GuidValue = new SchemaField(7, "GuidValue");
        }
        
        public static readonly SchemaField[] OrderedFields = new[]
        {
            Fields.StructureId,
            Fields.MemberPath,
            Fields.StringValue,
            Fields.IntegerValue,
            Fields.FractalValue,
            Fields.DateTimeValue,
            Fields.BoolValue,
            Fields.GuidValue
        };

        public static SchemaField GetValueSchemaFieldForType(Type dataType)
        {
            if (dataType.IsStringType())
                return Fields.StringValue;

            if (dataType.IsIntegerNumberType())
                return Fields.IntegerValue;

            if (dataType.IsFractalNumberType())
                return Fields.FractalValue;

            if (dataType.IsAnyDateTimeType())
                return Fields.DateTimeValue;

            if (dataType.IsAnyBoolType())
                return Fields.BoolValue;

            if (dataType.IsAnyGuidType())
                return Fields.GuidValue;

            throw new SisoDbException(ExceptionMessages.IndexStorageSchema_MissingValueSchemaField.Inject(dataType.Name));
        }

        public IndexStorageSchema(IStructureSchema structureSchema)
            : base(structureSchema, structureSchema.GetIndexesTableName())
        {
        }

        protected override SchemaField[] GetSchemaFields(IStructureSchema structureSchema)
        {
            return OrderedFields;
        }
    }
}