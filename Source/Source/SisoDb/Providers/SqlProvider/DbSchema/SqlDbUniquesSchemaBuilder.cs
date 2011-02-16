using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    public class SqlDbUniquesSchemaBuilder : ISqlDbSchemaBuilder
    {
        private readonly ISqlStrings _sqlStrings;

        public SqlDbUniquesSchemaBuilder(ISqlStrings sqlStrings)
        {
            _sqlStrings = sqlStrings;
        }

        public string GenerateSql(IStructureSchema structureSchema)
        {
            var sql = structureSchema.IdAccessor.IdType == IdTypes.Guid
                          ? _sqlStrings.GetSql("CreateUniquesGuid")
                          : _sqlStrings.GetSql("CreateUniquesIdentity");

            return sql.Inject(
                structureSchema.GetStructureTableName(),
                structureSchema.GetUniquesTableName());
        }
    }
}