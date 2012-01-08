using System;
using NCore.Reflections;
using PineCone.Structures.Schemas;

namespace SisoDb.DbSchema
{
	public class IndexesTableNames
	{
		private static readonly Type TextType;

		public string[] AllTableNames { get; private set; }
		public string IntegersTableName { get; private set; }
		public string FractalsTableName { get; private set; }
		public string DatesTableName { get; private set; }
		public string BooleansTableName { get; private set; }
		public string GuidsTableName { get; private set; }
		public string StringsTableName { get; private set; }
		public string TextsTableName { get; private set; }

		static IndexesTableNames()
		{
			TextType = typeof (Text);
		}

		public IndexesTableNames(IStructureSchema structureSchema)
		{
			IntegersTableName = structureSchema.GetIndexesTableNameFor(IndexesTypes.Integers);
			FractalsTableName = structureSchema.GetIndexesTableNameFor(IndexesTypes.Fractals);
			BooleansTableName = structureSchema.GetIndexesTableNameFor(IndexesTypes.Booleans);
			DatesTableName = structureSchema.GetIndexesTableNameFor(IndexesTypes.Dates);
			GuidsTableName = structureSchema.GetIndexesTableNameFor(IndexesTypes.Guids);
			StringsTableName = structureSchema.GetIndexesTableNameFor(IndexesTypes.Strings);
			TextsTableName = structureSchema.GetIndexesTableNameFor(IndexesTypes.Texts);

			AllTableNames = new[]
			{
				IntegersTableName, 
				FractalsTableName,
				BooleansTableName,
				DatesTableName, 
				GuidsTableName, 
				StringsTableName,
				TextsTableName
			};
		}

		public string GetNameByType(Type dataType)
		{
			if (dataType == TextType)
				return TextsTableName;

			if (dataType.IsAnyIntegerNumberType())
				return IntegersTableName;

			if (dataType.IsAnyFractalNumberType())
				return FractalsTableName;

			if (dataType.IsAnyBoolType())
				return BooleansTableName;

			if (dataType.IsAnyDateTimeType())
				return DatesTableName;

			if (dataType.IsAnyGuidType())
				return GuidsTableName;

			return StringsTableName;
		}
	}
}