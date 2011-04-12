using System;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    [Serializable]
    public class UniqueStorageSchema : StorageSchemaBase
    {
        public static class Fields
        {
            public static readonly SchemaField SisoIdRef = new SchemaField(0, "SisoIdRef");
            public static readonly SchemaField SisoId = new SchemaField(1, "SisoId");
            public static readonly SchemaField Name = new SchemaField(2, "Name");
            public static readonly SchemaField Value = new SchemaField(3, "Value");
            private static readonly SchemaField[] OrderedFields = new[] { SisoIdRef, SisoId, Name, Value };

            public static SchemaField[] GetOrderedFields()
            {
                return OrderedFields;
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