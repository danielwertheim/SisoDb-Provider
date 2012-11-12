using SisoDb.Dac;
using SisoDb.NCore;
using SisoDb.Resources;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSchema
{
    public class SqlDbUniquesSchemaBuilder
    {
        private readonly ISqlStatements _sqlStatements;

        public SqlDbUniquesSchemaBuilder(ISqlStatements sqlStatements)
        {
            _sqlStatements = sqlStatements;
        }

        public string GenerateSql(IStructureSchema structureSchema)
        {
            var uniquesTableName = structureSchema.GetUniquesTableName();
            var structureTableName = structureSchema.GetStructureTableName();

            if (structureSchema.IdAccessor.IdType == StructureIdTypes.String)
                return _sqlStatements.GetSql("CreateUniquesString").Inject(uniquesTableName, structureTableName);

            if (structureSchema.IdAccessor.IdType == StructureIdTypes.Guid)
                return _sqlStatements.GetSql("CreateUniquesGuid").Inject(uniquesTableName, structureTableName);

            if (structureSchema.IdAccessor.IdType.IsIdentity())
                return _sqlStatements.GetSql("CreateUniquesIdentity").Inject(uniquesTableName, structureTableName);

            throw new SisoDbException(ExceptionMessages.SqlDbUniquesSchemaBuilder_GenerateSql.Inject(structureSchema.IdAccessor.IdType));
        }
    }
}