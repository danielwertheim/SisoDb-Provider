using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Resources;
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
            var indexesTableName = structureSchema.GetIndexesTableName();
            var structureTableName = structureSchema.GetStructureTableName();

            if (structureSchema.IdAccessor.IdType == StructureIdTypes.String)
                return _sqlStatements.GetSql("CreateIndexesString").Inject(indexesTableName, structureTableName);

            if (structureSchema.IdAccessor.IdType == StructureIdTypes.Guid)
                return _sqlStatements.GetSql("CreateIndexesGuid").Inject(indexesTableName, structureTableName);

            if (structureSchema.IdAccessor.IdType.IsIdentity())
                return _sqlStatements.GetSql("CreateIndexesIdentity").Inject(indexesTableName, structureTableName);

            throw new SisoDbException(ExceptionMessages.SqlDbIndexesSchemaBuilder_GenerateSql.Inject(structureSchema.IdAccessor.IdType));
        }
    }
}