using System;

namespace SisoDb.DbSchema
{
    [Serializable]
	public class IndexesTableStatuses
	{
        public bool AllExists { get; private set; }
		public bool IntegersTableExists { get; private set; }
		public bool FractalsTableExists { get; private set; }
		public bool DatesTableExists { get; private set; }
		public bool BooleansTableExists { get; private set; }
		public bool GuidsTableExists { get; private set; }
		public bool StringsTableExists { get; private set; }
		public bool TextsTableExists { get; private set; }

		public IndexesTableStatuses( 
            bool integersTableExists, 
            bool fractalsTableExists, 
            bool datesTableExists, 
            bool booleansTableExists, 
            bool guidsTableExists, 
            bool stringsTableExists, 
            bool textsTableExists)
		{
		    IntegersTableExists = integersTableExists;
		    FractalsTableExists = fractalsTableExists;
		    DatesTableExists = datesTableExists;
		    BooleansTableExists = booleansTableExists;
		    GuidsTableExists = guidsTableExists;
		    StringsTableExists = stringsTableExists;
		    TextsTableExists = textsTableExists;

            AllExists = IntegersTableExists 
                && FractalsTableExists
                && DatesTableExists 
                && BooleansTableExists
                && GuidsTableExists
                && StringsTableExists
                && TextsTableExists;
		}
	}
}