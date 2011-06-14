using System;
using SisoDb.Core;
using SisoDb.Providers.DbSchema;
using SisoDb.Resources;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.Sql2008.DbSchema
{
    public class SqlDbStructuresSchemaBuilder : IDbSchemaBuilder
    {
        private readonly ISqlStatements _sqlStatements;

        public SqlDbStructuresSchemaBuilder(ISqlStatements sqlStatements)
        {
            _sqlStatements = sqlStatements;
        }

        public string GenerateSql(IStructureSchema structureSchema)
        {
            var isValidIdType = structureSchema.IdAccessor.IdType == IdTypes.Guid || structureSchema.IdAccessor.IdType == IdTypes.Identity;
            if (!isValidIdType)
                throw new NotSupportedException(
                   ExceptionMessages.DbSchemaUpserter_Upsert_IdTypeNotSupported.Inject(structureSchema.IdAccessor.IdType));

            var tableName = structureSchema.GetStructureTableName();
            var sql = structureSchema.IdAccessor.IdType == IdTypes.Guid
                          ? _sqlStatements.GetSql("CreateStructuresGuid")
                          : _sqlStatements.GetSql("CreateStructuresIdentity");

            return sql.Inject(tableName);
        }
    }
}