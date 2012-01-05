using System;
using NCore.Reflections;
using PineCone.Structures.Schemas;

namespace SisoDb.DbSchema
{
	public class IndexesTableNames
	{
		public string[] AllTableNames { get; private set; }
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
			BooleansTableName = structureSchema.GetIndexesTableNameFor(IndexesTypes.Booleans);
			DatesTableName = structureSchema.GetIndexesTableNameFor(IndexesTypes.Dates);
			GuidsTableName = structureSchema.GetIndexesTableNameFor(IndexesTypes.Guids);
			StringsTableName = structureSchema.GetIndexesTableNameFor(IndexesTypes.Strings);

			AllTableNames = new[]
			{
				IntegersTableName, 
				FractalsTableName,
				BooleansTableName,
				DatesTableName, 
				GuidsTableName, 
				StringsTableName
			};
		}

		public string GetNameByType(Type dataType)
		{
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