using System;
using EnsureThat;
using NCore.Reflections;
using PineCone.Structures.Schemas;

namespace SisoDb.DbSchema
{
    [Serializable]
    public class IndexesTableNames
	{
		private static readonly Type TextType;

	    public string this[int i]
	    {
            get { return AllTableNames[i]; }
	    }

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

        public IndexesTableNames(string structureName)
        {
            Ensure.That(structureName, "structureName").IsNotNullOrWhiteSpace();

            OnInitialize(structureName);
        }

	    public IndexesTableNames(IStructureSchema structureSchema)
	    {
	        Ensure.That(structureSchema, "structureSchema").IsNotNull();

            OnInitialize(structureSchema.Name);
	    }

	    private void OnInitialize(string structureName)
	    {
	        IntegersTableName = DbSchemas.GenerateIndexesTableNameFor(structureName, IndexesTypes.Integers);
	        FractalsTableName = DbSchemas.GenerateIndexesTableNameFor(structureName, IndexesTypes.Fractals);
	        BooleansTableName = DbSchemas.GenerateIndexesTableNameFor(structureName, IndexesTypes.Booleans);
	        DatesTableName = DbSchemas.GenerateIndexesTableNameFor(structureName, IndexesTypes.Dates);
	        GuidsTableName = DbSchemas.GenerateIndexesTableNameFor(structureName, IndexesTypes.Guids);
	        StringsTableName = DbSchemas.GenerateIndexesTableNameFor(structureName, IndexesTypes.Strings);
	        TextsTableName = DbSchemas.GenerateIndexesTableNameFor(structureName, IndexesTypes.Texts);

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