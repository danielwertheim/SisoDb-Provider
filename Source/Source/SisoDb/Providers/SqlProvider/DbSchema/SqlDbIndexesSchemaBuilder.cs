using System.Linq;
using SisoDb.Providers.Shared.DbSchema;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    public class SqlDbIndexesSchemaBuilder : ISqlDbSchemaBuilder
    {
        private readonly ISqlStrings _sqlStrings;
        private readonly SqlDbDataTypeTranslator _dataTypeTranslator;
        private readonly IDbColumnGenerator _columnGenerator;

        public SqlDbIndexesSchemaBuilder(ISqlStrings sqlStrings, IDbColumnGenerator columnGenerator)
        {
            _sqlStrings = sqlStrings.AssertNotNull("sqlStrings");
            _columnGenerator = columnGenerator.AssertNotNull("columnGenerator");
            _dataTypeTranslator = new SqlDbDataTypeTranslator();
        }

        public string GenerateSql(IStructureSchema structureSchema)
        {
            var columnDefinitions = structureSchema.IndexAccessors
                .Select(GenerateColumnDefinition);
            var columnsString = string.Join(",", columnDefinitions);
            var sql = structureSchema.IdAccessor.IdType == IdTypes.Guid
                          ? _sqlStrings.GetSql("CreateIndexesGuid")
                          : _sqlStrings.GetSql("CreateIndexesIdentity");

            return sql.Inject(
                structureSchema.GetStructureTableName(),
                structureSchema.GetIndexesTableName(),
                columnsString);
        }

        private string GenerateColumnDefinition(IIndexAccessor iac)
        {
            var dataTypeAsString = _dataTypeTranslator.ToDbType(iac);
            
            return _columnGenerator.ToSql(iac.Name, dataTypeAsString);
        }
    }
}