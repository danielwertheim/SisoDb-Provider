using System;
using System.Linq;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    [Serializable]
    internal class IndexStorageSchema : StorageSchemaBase
    {
        internal static class Fields
        {
            internal static SchemaField StructureId = new SchemaField(0, "StructureId");

            internal static SchemaField[] GetOrderedFields()
            {
                return new[] { StructureId };
            }
        }

        internal IndexStorageSchema(IStructureSchema structureSchema)
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