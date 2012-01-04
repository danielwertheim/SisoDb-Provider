using PineCone.Structures.Schemas;

namespace SisoDb.DbSchema
{
	public class IndexesTableNames
	{
		public string IntegersTableName { get; private set; }
		public string FractalsTableName { get; private set; }
		public string DatesTableName { get; private set; }
		public string BooleansTableName { get; private set; }
		public string GuidsTableName { get; private set; }
		public string StringsTableName { get; private set; }

		public IndexesTableNames(IStructureSchema structureSchema)
		{
			IntegersTableName = structureSchema.GetIndexesTableNameFor(IndexesTypes.Integers);
			FractalsTableName = structureSchema.GetIndexesTableNameFor(IndexesTypes.Fractals);
			DatesTableName = structureSchema.GetIndexesTableNameFor(IndexesTypes.Dates);
			BooleansTableName = structureSchema.GetIndexesTableNameFor(IndexesTypes.Booleans);
			GuidsTableName = structureSchema.GetIndexesTableNameFor(IndexesTypes.Guids);
			StringsTableName = structureSchema.GetIndexesTableNameFor(IndexesTypes.Strings);
		}
	}
}