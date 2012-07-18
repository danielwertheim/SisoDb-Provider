using System;
using System.Linq;
using SisoDb.Dac;
using SisoDb.NCore;
using SisoDb.PineCone.Structures;
using SisoDb.PineCone.Structures.Schemas;
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

		public string[] GenerateSql(IStructureSchema structureSchema, IndexesTableNames names, IndexesTableStatuses statuses)
		{
			if(statuses.AllExists)
				return new string[0];

			var structureTableName = structureSchema.GetStructureTableName();
			var sqlTemplateNameSuffix = GetSqlTemplateNameSuffix(structureSchema.IdAccessor.IdType);
			var generators = new Func<string>[]
			{
				() => !statuses.IntegersTableExists 
					? GenerateSqlFor("CreateIntegersIndexes", sqlTemplateNameSuffix, structureTableName, names.IntegersTableName) 
					: string.Empty,
				() => !statuses.FractalsTableExists 
					? GenerateSqlFor("CreateFractalsIndexes", sqlTemplateNameSuffix, structureTableName, names.FractalsTableName) 
					: string.Empty,
				() => !statuses.BooleansTableExists 
					? GenerateSqlFor("CreateBooleansIndexes", sqlTemplateNameSuffix, structureTableName, names.BooleansTableName) 
					: string.Empty,
				() => !statuses.DatesTableExists 
					? GenerateSqlFor("CreateDatesIndexes", sqlTemplateNameSuffix, structureTableName, names.DatesTableName) 
					: string.Empty,
				() => !statuses.GuidsTableExists 
					? GenerateSqlFor("CreateGuidsIndexes", sqlTemplateNameSuffix, structureTableName, names.GuidsTableName) 
					: string.Empty,
				() => !statuses.StringsTableExists 
					? GenerateSqlFor("CreateStringsIndexes", sqlTemplateNameSuffix, structureTableName, names.StringsTableName) 
					: string.Empty,
				() => !statuses.TextsTableExists 
					? GenerateSqlFor("CreateTextsIndexes", sqlTemplateNameSuffix, structureTableName, names.TextsTableName) 
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