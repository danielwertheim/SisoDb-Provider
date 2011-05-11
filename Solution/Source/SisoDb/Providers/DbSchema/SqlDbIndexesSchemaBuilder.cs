using System.Linq;
using SisoDb.Core;
using SisoDb.Providers.SqlStrings;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Providers.DbSchema
{
    public class SqlDbIndexesSchemaBuilder : IDbSchemaBuilder
    {
        private readonly ISqlStringsRepository _sqlStringsRepository;
        private readonly SqlDbDataTypeTranslator _dataTypeTranslator;
        private readonly IDbColumnGenerator _columnGenerator;

        public SqlDbIndexesSchemaBuilder(ISqlStringsRepository sqlStringsRepository, IDbColumnGenerator columnGenerator)
        {
            _sqlStringsRepository = sqlStringsRepository.AssertNotNull("sqlStringsRepository");
            _columnGenerator = columnGenerator.AssertNotNull("columnGenerator");
            _dataTypeTranslator = new SqlDbDataTypeTranslator();
        }

        public string GenerateSql(IStructureSchema structureSchema)
        {
            var columnDefinitions = structureSchema.IndexAccessors
                .Select(GenerateColumnDefinition);
            var columnsString = string.Join(",", columnDefinitions);
            var sql = structureSchema.IdAccessor.IdType == IdTypes.Guid
                          ? _sqlStringsRepository.GetSql("CreateIndexesGuid")
                          : _sqlStringsRepository.GetSql("CreateIndexesIdentity");

            return sql.Inject(
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