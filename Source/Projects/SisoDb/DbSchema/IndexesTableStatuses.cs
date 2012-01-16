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
	}
}