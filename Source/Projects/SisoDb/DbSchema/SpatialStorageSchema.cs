using System;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSchema
{
    [Serializable]
    public class SpatialStorageSchema : StorageSchemaBase
    {
        public static class Fields
        {
            public static readonly SchemaField RowId = new SchemaField(0, "RowId");
            public static readonly SchemaField Id = new SchemaField(1, "StructureId");
            public static readonly SchemaField Geo = new SchemaField(2, "Geo");
        }

        public static readonly SchemaField[] OrderedFields = new[] { Fields.RowId, Fields.Id, Fields.Geo };

        public SpatialStorageSchema(IStructureSchema structureSchema, string storageSchemaName) : base(structureSchema, storageSchemaName) {}
        protected override SchemaField[] GetSchemaFields(IStructureSchema structureSchema)
        {
            return OrderedFields;
        }
    }
}