using System;
using System.Linq;
using SisoDb.EnsureThat;

namespace SisoDb.DbSchema
{
    [Serializable]
    public class ModelTablesInfo
    {
        public ModelTableNames Names { get; private set; }
        public ModelTableStatuses Statuses { get; private set; }

        public ModelTablesInfo(ModelTableNames names, ModelTableStatuses statuses)
        {
            Ensure.That(names, "names").IsNotNull();
            Ensure.That(statuses, "statuses").IsNotNull();

            Names = names;
            Statuses = statuses;
        }

        public string[] GetIndexesTableNamesForExisting()
        {
            if (Statuses.IndexesTableStatuses.AllExists)
                return Names.IndexesTableNames.All;

            var names = new string[Names.IndexesTableNames.All.Length];

            if (Statuses.IndexesTableStatuses.IntegersTableExists)
                names[0] = Names.IndexesTableNames.IntegersTableName;

            if (Statuses.IndexesTableStatuses.FractalsTableExists)
                names[1] = Names.IndexesTableNames.FractalsTableName;

            if (Statuses.IndexesTableStatuses.DatesTableExists)
                names[2] = Names.IndexesTableNames.DatesTableName;

            if (Statuses.IndexesTableStatuses.BooleansTableExists)
                names[3] = Names.IndexesTableNames.BooleansTableName;

            if (Statuses.IndexesTableStatuses.GuidsTableExists)
                names[4] = Names.IndexesTableNames.GuidsTableName;

            if (Statuses.IndexesTableStatuses.StringsTableExists)
                names[5] = Names.IndexesTableNames.StringsTableName;

            if (Statuses.IndexesTableStatuses.TextsTableExists)
                names[6] = Names.IndexesTableNames.TextsTableName;

            return names.Where(n => n != null).ToArray();
        }
    }
}