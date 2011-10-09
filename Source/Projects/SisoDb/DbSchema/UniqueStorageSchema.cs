using System;
using PineCone.Structures.Schemas;
using SisoDb.Providers;

namespace SisoDb.DbSchema
{
    [Serializable]
    public class UniqueStorageSchema : StorageSchemaBase
    {
        public static class Fields
        {
            public static readonly SchemaField StructureId = new SchemaField(0, "StructureId");
            public static readonly SchemaField UqStructureId = new SchemaField(1, "UqStructureId");
            public static readonly SchemaField UqName = new SchemaField(2, "UqName");
            public static readonly SchemaField UqValue = new SchemaField(3, "UqValue");
        }

        public static readonly SchemaField[] OrderedFields = new[] { Fields.StructureId, Fields.UqStructureId, Fields.UqName, Fields.UqValue };

        public UniqueStorageSchema(IStructureSchema structureSchema)
            : base(structureSchema, structureSchema.GetUniquesTableName())
        {
        }

        protected override SchemaField[] GetSchemaFields(IStructureSchema structureSchema)
        {
            return OrderedFields;
        }
    }
}