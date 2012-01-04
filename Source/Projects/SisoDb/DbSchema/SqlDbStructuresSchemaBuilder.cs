using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Resources;

namespace SisoDb.DbSchema
{
	public class SqlDbStructuresSchemaBuilder : IDbSchemaBuilder
	{
		private readonly ISqlStatements _sqlStatements;

		public SqlDbStructuresSchemaBuilder(ISqlStatements sqlStatements)
		{
			_sqlStatements = sqlStatements;
		}

		public string[] GenerateSql(IStructureSchema structureSchema)
		{
			var tableName = structureSchema.GetStructureTableName();

			if (structureSchema.IdAccessor.IdType == StructureIdTypes.String)
				return new[] { _sqlStatements.GetSql("CreateStructuresString").Inject(tableName) };

			if (structureSchema.IdAccessor.IdType == StructureIdTypes.Guid)
				return new[] { _sqlStatements.GetSql("CreateStructuresGuid").Inject(tableName) };

			if (structureSchema.IdAccessor.IdType.IsIdentity())
				return new[] { _sqlStatements.GetSql("CreateStructuresIdentity").Inject(tableName) };

			throw new SisoDbException(ExceptionMessages.SqlDbStructureSchemaBuilder_GenerateSql.Inject(structureSchema.IdAccessor.IdType));
		}
	}
}