using System;
using System.Linq;
using PineCone.Structures.Schemas;
using SisoDb.Providers;

namespace SisoDb.DbSchema
{
    [Serializable]
    public class IndexStorageSchema : StorageSchemaBase
    {
        public static class Fields
        {
            public static readonly SchemaField StructureId = new SchemaField(0, "StructureId");
        }

        private static readonly SchemaField[] OrderedFields = new[] { Fields.StructureId };

        public IndexStorageSchema(IStructureSchema structureSchema)
            : base(structureSchema, structureSchema.GetIndexesTableName())
        {
        }

        protected override SchemaField[] GetSchemaFields(IStructureSchema structureSchema)
        {
            var staticFields = OrderedFields;
            var dynamicIndex = staticFields.Length;
            var dynamicFields = structureSchema.IndexAccessors.Select(iac => new SchemaField(dynamicIndex++, iac.Path)); //TODO: Cache
            
            return staticFields.Union(dynamicFields).ToArray(); //TODO: Hmmm, perhaps Merge???
        }
    }
}