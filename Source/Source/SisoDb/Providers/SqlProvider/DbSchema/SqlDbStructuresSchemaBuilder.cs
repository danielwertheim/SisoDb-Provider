using System;
using SisoDb.Resources;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    internal class SqlDbStructuresSchemaBuilder : ISqlDbSchemaBuilder
    {
        private readonly ISqlStrings _sqlStrings;

        internal SqlDbStructuresSchemaBuilder(ISqlStrings sqlStrings)
        {
            _sqlStrings = sqlStrings;
        }

        public string GenerateSql(IStructureSchema structureSchema)
        {
            var isValidIdType = structureSchema.IdAccessor.IdType == IdTypes.Guid || structureSchema.IdAccessor.IdType == IdTypes.Identity;
            if (!isValidIdType)
                throw new NotSupportedException(
                   ExceptionMessages.DbSchemaUpserter_Upsert_IdTypeNotSupported.Inject(structureSchema.IdAccessor.IdType));

            var tableName = structureSchema.GetStructureTableName();
            var sql = structureSchema.IdAccessor.IdType == IdTypes.Guid
                          ? _sqlStrings.GetSql("CreateStructuresGuid")
                          : _sqlStrings.GetSql("CreateStructuresIdentity");

            return sql.Inject(tableName);
        }
    }
}