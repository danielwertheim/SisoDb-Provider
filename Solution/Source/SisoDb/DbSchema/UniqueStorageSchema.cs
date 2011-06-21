using System;
using SisoDb.Providers;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSchema
{
    [Serializable]
    public class UniqueStorageSchema : StorageSchemaBase
    {
        public static class Fields
        {
            public static readonly SchemaField SisoId = new SchemaField(0, "SisoId");
            public static readonly SchemaField UqSisoId = new SchemaField(1, "UqSisoId");
            public static readonly SchemaField UqName = new SchemaField(2, "UqName");
            public static readonly SchemaField UqValue = new SchemaField(3, "UqValue");
            internal static readonly SchemaField[] OrderedFields = new[] { SisoId, UqSisoId, UqName, UqValue };
        }

        public UniqueStorageSchema(IStructureSchema structureSchema)
            : base(structureSchema, structureSchema.GetUniquesTableName())
        {
        }

        protected override SchemaField[] GetSchemaFields(IStructureSchema structureSchema)
        {
            return Fields.OrderedFields;
        }
    }
}