using System;
using System.Linq;
using SisoDb.Providers;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSchema
{
    [Serializable]
    public class IndexStorageSchema : StorageSchemaBase
    {
        public static class Fields
        {
            public static readonly SchemaField SisoId = new SchemaField(0, "SisoId");
            internal static readonly SchemaField[] OrderedFields = new[] { SisoId };
        }

        public IndexStorageSchema(IStructureSchema structureSchema)
            : base(structureSchema, structureSchema.GetIndexesTableName())
        {
        }

        protected override SchemaField[] GetSchemaFields(IStructureSchema structureSchema)
        {
            var staticFields = Fields.OrderedFields;
            var dynamicIndex = staticFields.Count();
            var dynamicFields = structureSchema.IndexAccessors.Select(iac => new SchemaField(dynamicIndex++, iac.Name)); //TODO: Cache
            
            return staticFields.Union(dynamicFields).ToArray();
        }
    }
}