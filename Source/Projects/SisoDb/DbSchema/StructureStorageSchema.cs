using System;
using PineCone.Structures.Schemas;
using SisoDb.Providers;
using SisoDb.Structures;

namespace SisoDb.DbSchema
{
    [Serializable]
    public class StructureStorageSchema : StorageSchemaBase
    {
        public static class Fields
        {
            public static readonly SchemaField Id = new SchemaField(0, StructureSchema.IdMemberName);
            public static readonly SchemaField Json = new SchemaField(1, "Json");
        }

        public static readonly SchemaField[] OrderedFields = new[] { Fields.Id, Fields.Json };

        public StructureStorageSchema(IStructureSchema structureSchema) 
            : base(structureSchema, structureSchema.GetStructureTableName())
        {
        }

        protected override SchemaField[] GetSchemaFields(IStructureSchema structureSchema)
        {
            return OrderedFields;
        }
    }
}