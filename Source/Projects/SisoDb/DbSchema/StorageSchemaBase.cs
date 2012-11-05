using System;
using System.Collections.Generic;
using System.Linq;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSchema
{
    [Serializable]
    public abstract class StorageSchemaBase
    {
        private readonly Dictionary<int, SchemaField> _fieldsByIndex;
        private readonly Dictionary<string, SchemaField> _fieldsByName;

        public SchemaField this[int index]
        {
            get { return _fieldsByIndex[index]; }
        }

        public SchemaField this[string name]
        {
            get { return _fieldsByName[name]; }
        }

        public string Name { get; private set; }

        protected StorageSchemaBase(IStructureSchema structureSchema, string storageSchemaName)
        {
            Name = storageSchemaName;

            _fieldsByIndex = new Dictionary<int, SchemaField>();
            _fieldsByName = new Dictionary<string, SchemaField>();
            
            InitializeFields(structureSchema);
        }

        private void InitializeFields(IStructureSchema structureSchema)
        {
            var fields = GetSchemaFields(structureSchema);

            foreach (var field in fields.OrderBy(f => f.Ordinal))
            {
                _fieldsByIndex.Add(field.Ordinal, field);
                _fieldsByName.Add(field.Name, field);
            }
        }

        protected abstract SchemaField[] GetSchemaFields(IStructureSchema structureSchema);

        public int GetFieldCount()
        {
            return _fieldsByIndex.Count;
        }

        public IEnumerable<SchemaField> GetFieldsOrderedByIndex()
        {
            return _fieldsByIndex.Values;
        }
    }
}