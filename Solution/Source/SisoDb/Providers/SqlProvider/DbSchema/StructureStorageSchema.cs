using System;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    [Serializable]
    public class StructureStorageSchema : StorageSchemaBase
    {
        public static class Fields
        {
            public static readonly SchemaField Id = new SchemaField(0, "SisoId");
            public static readonly SchemaField Json = new SchemaField(1, "Json");
            internal static readonly SchemaField[] OrderedFields = new[] { Id, Json };
        }

        public StructureStorageSchema(IStructureSchema structureSchema) 
            : base(structureSchema, structureSchema.GetStructureTableName())
        {
        }

        protected override SchemaField[] GetSchemaFields(IStructureSchema structureSchema)
        {
            return Fields.OrderedFields;
        }
    }
}