using System;
using System.Linq;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    [Serializable]
    public class IndexStorageSchema : StorageSchemaBase
    {
        public static class Fields
        {
            public static SchemaField SisoId = new SchemaField(0, "SisoId");

            public static SchemaField[] GetOrderedFields()
            {
                return new[] { SisoId };
            }
        }

        public IndexStorageSchema(IStructureSchema structureSchema)
            : base(structureSchema, structureSchema.GetIndexesTableName())
        {
        }

        protected override SchemaField[] GetSchemaFields(IStructureSchema structureSchema)
        {
            var staticFields = Fields.GetOrderedFields();
            var dynamicIndex = staticFields.Count();
            var dynamicFields = structureSchema.IndexAccessors.Select(iac => new SchemaField(dynamicIndex++, iac.Name));
            var fields = staticFields.Union(dynamicFields).ToArray();

            return fields;
        }
    }
}