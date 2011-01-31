using System;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    [Serializable]
    internal class StructureStorageSchema : StorageSchemaBase
    {
        internal static class Fields
        {
            internal static readonly SchemaField Id = new SchemaField(0, "Id");
            internal static readonly SchemaField Json = new SchemaField(1, "Json");

            internal static SchemaField[] GetOrderedFields()
            {
                return new[] { Id, Json };
            }
        }

        internal StructureStorageSchema(IStructureSchema structureSchema) 
            : base(structureSchema, structureSchema.GetStructureTableName())
        {
        }

        protected override SchemaField[] GetSchemaFields(IStructureSchema structureSchema)
        {
            return Fields.GetOrderedFields();
        }
    }
}