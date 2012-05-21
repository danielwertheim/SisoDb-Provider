using System;
using PineCone.Structures.Schemas;

namespace SisoDb.DbSchema
{
    [Serializable]
    public class IndexStorageSchema : StorageSchemaBase
    {
        public static class Fields
        {
            public static readonly SchemaField RowId = new SchemaField(0, "RowId");
            public static readonly SchemaField StructureId = new SchemaField(1, "StructureId");
            public static readonly SchemaField MemberPath = new SchemaField(2, "MemberPath");
            public static readonly SchemaField Value = new SchemaField(3, "Value");
			public static readonly SchemaField StringValue = new SchemaField(4, "StringValue");
        }
        
        public static readonly SchemaField[] OrderedFields = new[]
        {
            Fields.RowId,
            Fields.StructureId,
            Fields.MemberPath,
			Fields.Value,
            Fields.StringValue
        };

    	public IndexStorageSchema(IStructureSchema structureSchema, string storageSchemaName) 
			: base(structureSchema, storageSchemaName) {}
    	
		protected override SchemaField[] GetSchemaFields(IStructureSchema structureSchema)
		{
			return OrderedFields;
		}
    }
}