using System;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSchema
{
	[Serializable]
	public class UniqueStorageSchema : StorageSchemaBase
	{
		public static class Fields
		{
            public static readonly SchemaField RowId = new SchemaField(0, "RowId");
			public static readonly SchemaField StructureId = new SchemaField(1, "StructureId");
			public static readonly SchemaField UqStructureId = new SchemaField(2, "UqStructureId");
			public static readonly SchemaField UqMemberPath = new SchemaField(3, "UqMemberPath");
			public static readonly SchemaField UqValue = new SchemaField(4, "UqValue");
		}

		public static readonly SchemaField[] OrderedFields = new[]
		{
		    Fields.RowId, 
            Fields.StructureId, 
            Fields.UqStructureId, 
            Fields.UqMemberPath, 
            Fields.UqValue
		};

		public UniqueStorageSchema(IStructureSchema structureSchema, string storageSchemaName) 
			: base(structureSchema, storageSchemaName)
		{
		}

		protected override SchemaField[] GetSchemaFields(IStructureSchema structureSchema)
		{
			return OrderedFields;
		}
	}
}