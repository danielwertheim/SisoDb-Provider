using System;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    [Serializable]
    internal class UniqueStorageSchema : StorageSchemaBase
    {
        internal static class Fields
        {
            internal static SchemaField StructureId = new SchemaField(0, "StructureId");
            internal static SchemaField Name = new SchemaField(1, "Name");
            internal static SchemaField Value = new SchemaField(2, "Value");

            internal static SchemaField[] GetOrderedFields()
            {
                return new[] { StructureId, Name, Value };
            }
        }

        internal UniqueStorageSchema(IStructureSchema structureSchema)
            : base(structureSchema, structureSchema.GetUniquesTableName())
        {
        }

        protected override SchemaField[] GetSchemaFields(IStructureSchema structureSchema)
        {
            return Fields.GetOrderedFields();
        }
    }
}