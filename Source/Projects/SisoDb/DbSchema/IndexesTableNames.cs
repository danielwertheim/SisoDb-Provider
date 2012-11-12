using System;
using SisoDb.EnsureThat;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSchema
{
    [Serializable]
    public class IndexesTableNames
    {
        public string this[int i]
        {
            get { return All[i]; }
        }

        public string[] All { get; private set; }
        public string IntegersTableName { get; private set; }
        public string FractalsTableName { get; private set; }
        public string DatesTableName { get; private set; }
        public string BooleansTableName { get; private set; }
        public string GuidsTableName { get; private set; }
        public string StringsTableName { get; private set; }
        public string TextsTableName { get; private set; }

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
            IntegersTableName = DbSchemaInfo.GenerateIndexesTableNameFor(structureName, IndexesTypes.Integers);
            FractalsTableName = DbSchemaInfo.GenerateIndexesTableNameFor(structureName, IndexesTypes.Fractals);
            BooleansTableName = DbSchemaInfo.GenerateIndexesTableNameFor(structureName, IndexesTypes.Booleans);
            DatesTableName = DbSchemaInfo.GenerateIndexesTableNameFor(structureName, IndexesTypes.Dates);
            GuidsTableName = DbSchemaInfo.GenerateIndexesTableNameFor(structureName, IndexesTypes.Guids);
            StringsTableName = DbSchemaInfo.GenerateIndexesTableNameFor(structureName, IndexesTypes.Strings);
            TextsTableName = DbSchemaInfo.GenerateIndexesTableNameFor(structureName, IndexesTypes.Texts);

            All = new[]
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

        public string GetNameByType(DataTypeCode dataTypeCode)
        {
            switch (dataTypeCode)
            {
                case DataTypeCode.IntegerNumber:
                    return IntegersTableName;
                case DataTypeCode.FractalNumber:
                    return FractalsTableName;
                case DataTypeCode.Bool:
                    return BooleansTableName;
                case DataTypeCode.DateTime:
                    return DatesTableName;
                case DataTypeCode.Guid:
                    return GuidsTableName;
                case DataTypeCode.String:
                    return StringsTableName;
                case DataTypeCode.Text:
                    return TextsTableName;
            }

            return StringsTableName;
        }
    }
}