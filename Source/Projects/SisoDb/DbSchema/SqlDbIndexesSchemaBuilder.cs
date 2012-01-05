using System;
using System.Linq;
using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Resources;

namespace SisoDb.DbSchema
{
	public class SqlDbIndexesSchemaBuilder
	{
		private readonly ISqlStatements _sqlStatements;

		public SqlDbIndexesSchemaBuilder(ISqlStatements sqlStatements)
		{
			_sqlStatements = sqlStatements;
		}

		public string[] GenerateSql(IStructureSchema structureSchema, IndexesTableStatuses indexesTableStatuses)
		{
			if(indexesTableStatuses.AllExists)
				return new string[]{};

			var structureTableName = structureSchema.GetStructureTableName();
			var sqlTemplateNameSuffix = GetSqlTemplateNameSuffix(structureSchema.IdAccessor.IdType);
			var generators = new Func<string>[]
			{
				() => !indexesTableStatuses.IntegersTableExists 
					? GenerateSqlForIntegers(sqlTemplateNameSuffix, structureTableName, indexesTableStatuses.Names.IntegersTableName) 
					: string.Empty,
				() => !indexesTableStatuses.FractalsTableExists 
					? GenerateSqlForFractals(sqlTemplateNameSuffix, structureTableName, indexesTableStatuses.Names.FractalsTableName) 
					: string.Empty,
				() => !indexesTableStatuses.BooleansTableExists 
					? GenerateSqlForBooleans(sqlTemplateNameSuffix, structureTableName, indexesTableStatuses.Names.BooleansTableName) 
					: string.Empty,
				() => !indexesTableStatuses.DatesTableExists 
					? GenerateSqlForDates(sqlTemplateNameSuffix, structureTableName, indexesTableStatuses.Names.DatesTableName) 
					: string.Empty,
				() => !indexesTableStatuses.GuidsTableExists 
					? GenerateSqlForGuids(sqlTemplateNameSuffix, structureTableName, indexesTableStatuses.Names.GuidsTableName) 
					: string.Empty,
				() => !indexesTableStatuses.StringsTableExists 
					? GenerateSqlForStrings(sqlTemplateNameSuffix, structureTableName, indexesTableStatuses.Names.StringsTableName) 
					: string.Empty
			};

			return generators.Select(generator => generator()).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
		}

		private string GetSqlTemplateNameSuffix(StructureIdTypes idType)
		{
			if(idType == StructureIdTypes.String)
				return "String";

			if (idType == StructureIdTypes.Guid)
				return "Guid";

			if (idType.IsIdentity())
				return "Identity";

			throw new SisoDbException(ExceptionMessages.SqlDbIndexesSchemaBuilder_GenerateSql.Inject(idType));
		}

		private string GenerateSqlForIntegers(string sqlTemplateNameSuffix, string structureTableName, string indexesTableName)
		{
			return string.Format(
				_sqlStatements.GetSql(string.Concat("CreateIntegersIndexes", sqlTemplateNameSuffix)),
				indexesTableName,
				structureTableName);
		}

		private string GenerateSqlForFractals(string sqlTemplateNameSuffix, string structureTableName, string indexesTableName)
		{
			return string.Format(
				_sqlStatements.GetSql(string.Concat("CreateFractalsIndexes", sqlTemplateNameSuffix)),
				indexesTableName,
				structureTableName);
		}

		private string GenerateSqlForBooleans(string sqlTemplateNameSuffix, string structureTableName, string indexesTableName)
		{
			return string.Format(
				_sqlStatements.GetSql(string.Concat("CreateBooleansIndexes", sqlTemplateNameSuffix)),
				indexesTableName,
				structureTableName);
		}

		private string GenerateSqlForDates(string sqlTemplateNameSuffix, string structureTableName, string indexesTableName)
		{
			return string.Format(
				_sqlStatements.GetSql(string.Concat("CreateDatesIndexes", sqlTemplateNameSuffix)),
				indexesTableName,
				structureTableName);
		}

		private string GenerateSqlForGuids(string sqlTemplateNameSuffix, string structureTableName, string indexesTableName)
		{
			return string.Format(
				_sqlStatements.GetSql(string.Concat("CreateGuidsIndexes", sqlTemplateNameSuffix)),
				indexesTableName,
				structureTableName);
		}

		private string GenerateSqlForStrings(string sqlTemplateNameSuffix, string structureTableName, string indexesTableName)
		{
			return string.Format(
				_sqlStatements.GetSql(string.Concat("CreateStringsIndexes", sqlTemplateNameSuffix)),
				indexesTableName,
				structureTableName);
		}
	}
}