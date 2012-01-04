using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Resources;

namespace SisoDb.DbSchema
{
    public class SqlDbUniquesSchemaBuilder : IDbSchemaBuilder
    {
        private readonly ISqlStatements _sqlStatements;

        public SqlDbUniquesSchemaBuilder(ISqlStatements sqlStatements)
        {
            _sqlStatements = sqlStatements;
        }

        public string[] GenerateSql(IStructureSchema structureSchema)
        {
            var uniquesTableName = structureSchema.GetUniquesTableName();
            var structureTableName = structureSchema.GetStructureTableName();

            if (structureSchema.IdAccessor.IdType == StructureIdTypes.String)
                return new[] { _sqlStatements.GetSql("CreateUniquesString").Inject(uniquesTableName, structureTableName) };

            if (structureSchema.IdAccessor.IdType == StructureIdTypes.Guid)
                return new[] { _sqlStatements.GetSql("CreateUniquesGuid").Inject(uniquesTableName, structureTableName) };

            if (structureSchema.IdAccessor.IdType.IsIdentity())
                return new[] { _sqlStatements.GetSql("CreateUniquesIdentity").Inject(uniquesTableName, structureTableName) };

            throw new SisoDbException(ExceptionMessages.SqlDbUniquesSchemaBuilder_GenerateSql.Inject(structureSchema.IdAccessor.IdType));
        }
    }
}