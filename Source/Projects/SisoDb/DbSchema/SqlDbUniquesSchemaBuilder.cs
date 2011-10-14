using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Providers;
using SisoDb.Structures;

namespace SisoDb.DbSchema
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