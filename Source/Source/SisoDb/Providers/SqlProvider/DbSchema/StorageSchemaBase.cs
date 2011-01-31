using System;
using System.Collections.Generic;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    [Serializable]
    internal abstract class StorageSchemaBase
    {
        internal readonly Dictionary<int, SchemaField> FieldsByIndex;
        internal readonly Dictionary<string, SchemaField> FieldsByName;

        internal string Name { get; private set; }

        protected StorageSchemaBase(IStructureSchema structureSchema, string storageSchemaName)
        {
            FieldsByIndex = new Dictionary<int, SchemaField>();
            FieldsByName = new Dictionary<string, SchemaField>();
            Name = storageSchemaName;
            
            InitializeFields(structureSchema);
        }

        protected void InitializeFields(IStructureSchema structureSchema)
        {
            var fields = GetSchemaFields(structureSchema);

            foreach (var field in fields)
            {
                FieldsByIndex.Add(field.Ordinal, field);
                FieldsByName.Add(field.Name, field);
            }
        }

        protected abstract SchemaField[] GetSchemaFields(IStructureSchema structureSchema);

        internal int FieldCount()
        {
            return FieldsByIndex.Count;
        }
    }
}