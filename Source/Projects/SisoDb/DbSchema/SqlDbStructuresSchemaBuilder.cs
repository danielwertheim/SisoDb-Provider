using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.NCore;
using SisoDb.Resources;

namespace SisoDb.DbSchema
{
	public class SqlDbStructuresSchemaBuilder
	{
		private readonly ISqlStatements _sqlStatements;

		public SqlDbStructuresSchemaBuilder(ISqlStatements sqlStatements)
		{
			_sqlStatements = sqlStatements;
		}

		public string GenerateSql(IStructureSchema structureSchema)
		{
			var tableName = structureSchema.GetStructureTableName();

			if (structureSchema.IdAccessor.IdType == StructureIdTypes.String)
				return _sqlStatements.GetSql("CreateStructuresString").Inject(tableName);

			if (structureSchema.IdAccessor.IdType == StructureIdTypes.Guid)
				return _sqlStatements.GetSql("CreateStructuresGuid").Inject(tableName);

			if (structureSchema.IdAccessor.IdType.IsIdentity())
				return _sqlStatements.GetSql("CreateStructuresIdentity").Inject(tableName);

			throw new SisoDbException(ExceptionMessages.SqlDbStructureSchemaBuilder_GenerateSql.Inject(structureSchema.IdAccessor.IdType));
		}
	}
}