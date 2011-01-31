using System.Linq;
using SisoDb.Providers.AzureProvider.DbSchema;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    internal class SqlDbIndexesSchemaBuilder : ISqlDbSchemaBuilder
    {
        private readonly ISqlStrings _sqlStrings;
        private readonly SqlDbDataTypeTranslator _dataTypeTranslator;
        private readonly ISqlDbColumnGenerator _columnGenerator;

        internal SqlDbIndexesSchemaBuilder(ISqlStrings sqlStrings, StorageProviders providerType)
        {
            _sqlStrings = sqlStrings;
            _dataTypeTranslator = new SqlDbDataTypeTranslator();

            //TODO: IoC or Factory
            _columnGenerator = providerType == StorageProviders.Sql2008 ?
                (ISqlDbColumnGenerator)new SqlDbColumnGenerator() : new AzureDbColumnGenerator();
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