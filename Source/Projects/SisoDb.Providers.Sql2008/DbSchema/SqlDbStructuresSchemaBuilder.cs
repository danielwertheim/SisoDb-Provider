using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.DbSchema;
using SisoDb.Providers;

namespace SisoDb.Sql2008.DbSchema
{
    public class SqlDbStructuresSchemaBuilder : IDbSchemaBuilder
    {
        private readonly ISqlStatements _sqlStatements;

        public SqlDbStructuresSchemaBuilder(ISqlStatements sqlStatements)
        {
            _sqlStatements = sqlStatements;
        }

        public string GenerateSql(IStructureSchema structureSchema)
        {
            var tableName = structureSchema.GetStructureTableName();
            var sql = structureSchema.IdAccessor.IdType == StructureIdTypes.Guid
                          ? _sqlStatements.GetSql("CreateStructuresGuid")
                          : _sqlStatements.GetSql("CreateStructuresIdentity");

            return sql.Inject(tableName);
        }
    }
}