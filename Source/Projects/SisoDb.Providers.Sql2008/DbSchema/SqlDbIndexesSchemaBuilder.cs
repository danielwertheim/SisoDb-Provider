using System.Linq;
using EnsureThat;
using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using PineCone.Structures.Schemas.MemberAccessors;
using SisoDb.DbSchema;
using SisoDb.Providers;

namespace SisoDb.Sql2008.DbSchema
{
    public class SqlDbIndexesSchemaBuilder : IDbSchemaBuilder
    {
        private readonly ISqlStatements _sqlStatements;
        private readonly Sql2008DataTypeTranslator _dataTypeTranslator;
        private readonly IDbColumnGenerator _columnGenerator;

        public SqlDbIndexesSchemaBuilder(ISqlStatements sqlStatements, IDbColumnGenerator columnGenerator)
        {
            Ensure.That(() => sqlStatements).IsNotNull();
            Ensure.That(() => columnGenerator).IsNotNull();

            _sqlStatements = sqlStatements;
            _columnGenerator = columnGenerator;
            _dataTypeTranslator = new Sql2008DataTypeTranslator();
        }

        public string GenerateSql(IStructureSchema structureSchema)
        {
            var columnDefinitions = structureSchema.IndexAccessors
                .Select(GenerateColumnDefinition);
            var columnsString = string.Join(",", columnDefinitions);
            var sql = structureSchema.IdAccessor.IdType == StructureIdTypes.Guid
                          ? _sqlStatements.GetSql("CreateIndexesGuid")
                          : _sqlStatements.GetSql("CreateIndexesIdentity");

            return sql.Inject(
                structureSchema.GetIndexesTableName(),
                structureSchema.GetStructureTableName(),
                columnsString);
        }

        private string GenerateColumnDefinition(IIndexAccessor iac)
        {
            var dataTypeAsString = _dataTypeTranslator.ToDbType(iac);
            
            return _columnGenerator.ToSql(iac.Path, dataTypeAsString);
        }
    }
}