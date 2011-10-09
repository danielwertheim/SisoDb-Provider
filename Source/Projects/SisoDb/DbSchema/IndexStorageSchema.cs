using System;
using System.Linq;
using PineCone.Structures.Schemas;
using SisoDb.Providers;

namespace SisoDb.DbSchema
{
    [Serializable]
    public class IndexStorageSchema : StorageSchemaBase
    {
        public static class Fields
        {
            public static readonly SchemaField StructureId = new SchemaField(0, "StructureId");
            public static readonly SchemaField MemberPath = new SchemaField(1, "MemberPath");
            public static readonly SchemaField StringValue = new SchemaField(2, "StringValue");
            public static readonly SchemaField IntegerValue = new SchemaField(3, "IntegerValue");
            public static readonly SchemaField FractalValue = new SchemaField(4, "FractalValue");
            public static readonly SchemaField DateTimeValue = new SchemaField(5, "DateTimeValue");
            public static readonly SchemaField BitValue = new SchemaField(6, "BitValue");
        }

        public static readonly SchemaField[] OrderedFields = new[] { Fields.StructureId, Fields.MemberPath, Fields.StringValue, Fields.IntegerValue, Fields.FractalValue, Fields.DateTimeValue, Fields.BitValue };

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