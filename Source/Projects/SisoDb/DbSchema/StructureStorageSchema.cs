using System;
using PineCone.Structures.Schemas;

namespace SisoDb.DbSchema
{
    [Serializable]
    public class StructureStorageSchema : StorageSchemaBase
    {
        public static class Fields
        {
            public static readonly SchemaField Id = new SchemaField(0, "StructureId");
            public static readonly SchemaField Json = new SchemaField(1, "Json");
            public static readonly SchemaField RowId = new SchemaField(2, "RowId");
        }

        public static readonly SchemaField[] OrderedFields = new[] { Fields.Id, Fields.Json, Fields.RowId };

		public StructureStorageSchema(IStructureSchema structureSchema, string storageSchemaName)
			: base(structureSchema, storageSchemaName)
		{
		}

        protected override SchemaField[] GetSchemaFields(IStructureSchema structureSchema)
        {
            return OrderedFields;
        }
    }
}