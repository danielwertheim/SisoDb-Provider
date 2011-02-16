using System;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    [Serializable]
    public class UniqueStorageSchema : StorageSchemaBase
    {
        public static class Fields
        {
            public static SchemaField StructureId = new SchemaField(0, "StructureId");
            public static SchemaField Name = new SchemaField(1, "Name");
            public static SchemaField Value = new SchemaField(2, "Value");

            public static SchemaField[] GetOrderedFields()
            {
                return new[] { StructureId, Name, Value };
            }
        }

        public UniqueStorageSchema(IStructureSchema structureSchema)
            : base(structureSchema, structureSchema.GetUniquesTableName())
        {
        }

        protected override SchemaField[] GetSchemaFields(IStructureSchema structureSchema)
        {
            return Fields.GetOrderedFields();
        }
    }
}