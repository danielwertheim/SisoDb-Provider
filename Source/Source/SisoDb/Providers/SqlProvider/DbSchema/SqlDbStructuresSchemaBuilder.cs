using System;
using SisoDb.Core;
using SisoDb.Providers.DbSchema;
using SisoDb.Providers.SqlStrings;
using SisoDb.Resources;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    public class SqlDbStructuresSchemaBuilder : IDbSchemaBuilder
    {
        private readonly ISqlStringsRepository _sqlStringsRepository;

        public SqlDbStructuresSchemaBuilder(ISqlStringsRepository sqlStringsRepository)
        {
            _sqlStringsRepository = sqlStringsRepository;
        }

        public string GenerateSql(IStructureSchema structureSchema)
        {
            var isValidIdType = structureSchema.IdAccessor.IdType == IdTypes.Guid || structureSchema.IdAccessor.IdType == IdTypes.Identity;
            if (!isValidIdType)
                throw new NotSupportedException(
                   ExceptionMessages.DbSchemaUpserter_Upsert_IdTypeNotSupported.Inject(structureSchema.IdAccessor.IdType));

            var tableName = structureSchema.GetStructureTableName();
            var sql = structureSchema.IdAccessor.IdType == IdTypes.Guid
                          ? _sqlStringsRepository.GetSql("CreateStructuresGuid")
                          : _sqlStringsRepository.GetSql("CreateStructuresIdentity");

            return sql.Inject(tableName);
        }
    }
}