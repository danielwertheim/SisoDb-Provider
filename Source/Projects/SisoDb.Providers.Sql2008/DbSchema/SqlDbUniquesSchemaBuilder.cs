using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.DbSchema;
using SisoDb.Providers;

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
            var sql = structureSchema.IdAccessor.IdType == StructureIdTypes.Guid
                          ? _sqlStatements.GetSql("CreateUniquesGuid")
                          : _sqlStatements.GetSql("CreateUniquesIdentity");

            return sql.Inject(
                structureSchema.GetUniquesTableName(),
                structureSchema.GetStructureTableName());
        }
    }
}