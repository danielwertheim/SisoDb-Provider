﻿using SisoDb.Core;
using SisoDb.Providers.SqlStrings;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.DbSchema
{
    public class SqlDbUniquesSchemaBuilder : IDbSchemaBuilder
    {
        private readonly ISqlStringsRepository _sqlStringsRepository;

        public SqlDbUniquesSchemaBuilder(ISqlStringsRepository sqlStringsRepository)
        {
            _sqlStringsRepository = sqlStringsRepository;
        }

        public string GenerateSql(IStructureSchema structureSchema)
        {
            var sql = structureSchema.IdAccessor.IdType == IdTypes.Guid
                          ? _sqlStringsRepository.GetSql("CreateUniquesGuid")
                          : _sqlStringsRepository.GetSql("CreateUniquesIdentity");

            return sql.Inject(structureSchema.GetUniquesTableName());
        }
    }
}