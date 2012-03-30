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
				return new string[0];

			var structureTableName = structureSchema.GetStructureTableName();
			var sqlTemplateNameSuffix = GetSqlTemplateNameSuffix(structureSchema.IdAccessor.IdType);
			var generators = new Func<string>[]
			{
				() => !indexesTableStatuses.IntegersTableExists 
					? GenerateSqlFor("CreateIntegersIndexes", sqlTemplateNameSuffix, structureTableName, indexesTableStatuses.Names.IntegersTableName) 
					: string.Empty,
				() => !indexesTableStatuses.FractalsTableExists 
					? GenerateSqlFor("CreateFractalsIndexes", sqlTemplateNameSuffix, structureTableName, indexesTableStatuses.Names.FractalsTableName) 
					: string.Empty,
				() => !indexesTableStatuses.BooleansTableExists 
					? GenerateSqlFor("CreateBooleansIndexes", sqlTemplateNameSuffix, structureTableName, indexesTableStatuses.Names.BooleansTableName) 
					: string.Empty,
				() => !indexesTableStatuses.DatesTableExists 
					? GenerateSqlFor("CreateDatesIndexes", sqlTemplateNameSuffix, structureTableName, indexesTableStatuses.Names.DatesTableName) 
					: string.Empty,
				() => !indexesTableStatuses.GuidsTableExists 
					? GenerateSqlFor("CreateGuidsIndexes", sqlTemplateNameSuffix, structureTableName, indexesTableStatuses.Names.GuidsTableName) 
					: string.Empty,
				() => !indexesTableStatuses.StringsTableExists 
					? GenerateSqlFor("CreateStringsIndexes", sqlTemplateNameSuffix, structureTableName, indexesTableStatuses.Names.StringsTableName) 
					: string.Empty,
				() => !indexesTableStatuses.TextsTableExists 
					? GenerateSqlFor("CreateTextsIndexes", sqlTemplateNameSuffix, structureTableName, indexesTableStatuses.Names.TextsTableName) 
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

		private string GenerateSqlFor(string sqlTemplateName, string sqlTemplateNameSuffix, string structureTableName, string indexesTableName)
		{
			return string.Format(
				_sqlStatements.GetSql(string.Concat(sqlTemplateName, sqlTemplateNameSuffix)),
				indexesTableName,
				structureTableName);
		}
	}
}