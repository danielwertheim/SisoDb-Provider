using SisoDb.Core;
using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Sql2008.DbSchema
{
    public class SqlDbUniquesSchemaBuilder : IDbSchemaBuilder
    {
        private readonly ISqlStatements _sqlStatements;

        public SqlDbUniquesSchemaBuilder(ISqlStatements sqlStatements)
        {
            _sqlStatements = sqlStatements;
        }

        public string GenerateSql(IStructureSchema structureSchema)
        {
            var sql = structureSchema.IdAccessor.IdType == IdTypes.Guid
                          ? _sqlStatements.GetSql("CreateUniquesGuid")
                          : _sqlStatements.GetSql("CreateUniquesIdentity");

            return sql.Inject(
                structureSchema.GetUniquesTableName(),
                structureSchema.GetStructureTableName());
        }
    }
}