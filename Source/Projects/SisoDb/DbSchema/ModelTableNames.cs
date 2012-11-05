using System;
using System.Linq;
using SisoDb.EnsureThat;
using SisoDb.NCore.Collections;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSchema
{
    [Serializable]
    public class ModelTableNames
    {
        public string[] AllTableNames { get; private set; }
        public string StructureTableName { get; private set; }
        public string UniquesTableName { get; private set; }
        public IndexesTableNames IndexesTableNames { get; private set; }

        public ModelTableNames(string structureName)
        {
            Ensure.That(structureName, "structureName").IsNotNullOrWhiteSpace();

            OnInitialize(structureName);
        }

        public ModelTableNames(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            OnInitialize(structureSchema.Name);
        }

        private void OnInitialize(string structureName)
        {
            StructureTableName = DbSchemas.GenerateStructureTableName(structureName);
            UniquesTableName = DbSchemas.GenerateUniquesTableName(structureName);
            IndexesTableNames = new IndexesTableNames(structureName);

            AllTableNames = new[]
	        {
	            StructureTableName,
                UniquesTableName
	        }
            .MergeWith(IndexesTableNames.All)
            .ToArray();
        }
    }
}