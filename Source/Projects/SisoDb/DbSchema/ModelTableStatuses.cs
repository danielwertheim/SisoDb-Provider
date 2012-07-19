using System;
using SisoDb.EnsureThat;

namespace SisoDb.DbSchema
{
    [Serializable]
    public class ModelTableStatuses
    {
        public bool AllExists { get; private set; }
        public bool StructureTableExists { get; private set; }
        public bool UniquesTableExists { get; private set; }
        public IndexesTableStatuses IndexesTableStatuses { get; private set; }

        public ModelTableStatuses(bool structureTableExists, bool uniquesTableExists, IndexesTableStatuses indexesTableStatuses)
        {
            Ensure.That(indexesTableStatuses, "indexesTableStatuses").IsNotNull();

            StructureTableExists = structureTableExists;
            UniquesTableExists = uniquesTableExists;
            IndexesTableStatuses = indexesTableStatuses;

            AllExists = StructureTableExists
                        && UniquesTableExists
                        && IndexesTableStatuses.AllExists;
        }
    }
}