using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Providers;
using SisoDb.Structures;

namespace SisoDb.DbSchema
{
    public class SqlDbIndexesSchemaBuilder : IDbSchemaBuilder
    {
        private readonly ISqlStatements _sqlStatements;

        public SqlDbIndexesSchemaBuilder(ISqlStatements sqlStatements)
        {
            _sqlStatements = sqlStatements;
        }

        public string GenerateSql(IStructureSchema structureSchema)
        {
            var sql = structureSchema.IdAccessor.IdType == StructureIdTypes.Guid
                          ? _sqlStatements.GetSql("CreateIndexesGuid")
                          : _sqlStatements.GetSql("CreateIndexesIdentity");

            return sql.Inject(
                structureSchema.GetIndexesTableName(),
                structureSchema.GetStructureTableName());
        }
    }
}