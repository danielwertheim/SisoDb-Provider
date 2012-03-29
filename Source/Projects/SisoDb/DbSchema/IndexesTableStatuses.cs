using System.Linq;

namespace SisoDb.DbSchema
{
	public class IndexesTableStatuses
	{
		public IndexesTableNames Names { get; private set; }

		public bool AllExists
		{
			get
			{
				return IntegersTableExists &&
				       FractalsTableExists &&
				       DatesTableExists &&
				       BooleansTableExists &&
				       GuidsTableExists &&
				       StringsTableExists &&
					   TextsTableExists;
			}
		}

		public bool IntegersTableExists { get; set; }
		public bool FractalsTableExists { get; set; }
		public bool DatesTableExists { get; set; }
		public bool BooleansTableExists { get; set; }
		public bool GuidsTableExists { get; set; }
		public bool StringsTableExists { get; set; }
		public bool TextsTableExists { get; set; }

		public IndexesTableStatuses(IndexesTableNames names)
		{
			Names = names;
		}

        public string[] GetTableNamesForExisting()
        {
            if (AllExists)
                return Names.AllTableNames;

            var names = new string[Names.AllTableNames.Length];

            if (IntegersTableExists)
                names[0] = Names.IntegersTableName;

            if (FractalsTableExists)
                names[1] = Names.FractalsTableName;

            if (DatesTableExists)
                names[2] = Names.DatesTableName;

            if (BooleansTableExists)
                names[3] = Names.BooleansTableName;

            if (GuidsTableExists)
                names[4] = Names.GuidsTableName;

            if (StringsTableExists)
                names[5] = Names.StringsTableName;

            if (TextsTableExists)
                names[6] = Names.TextsTableName;

            return names.Where(n => n != null).ToArray();
        }
	}
}