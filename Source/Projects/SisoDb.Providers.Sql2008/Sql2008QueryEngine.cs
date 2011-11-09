using PineCone.Structures.Schemas;
using SisoDb.DbSchema;
using SisoDb.Serialization;

namespace SisoDb.Sql2008
{
    public class Sql2008QueryEngine : DbQueryEngine
    {
        internal Sql2008QueryEngine(
            ISisoConnectionInfo connectionInfo,
            IDbSchemaManager dbSchemaManager,
            IStructureSchemas structureSchemas,
            IJsonSerializer jsonSerializer) 
            : base(connectionInfo, false, dbSchemaManager, structureSchemas, jsonSerializer)
        {}

        protected Sql2008QueryEngine(
            ISisoConnectionInfo connectionInfo,
            bool transactional,
            IDbSchemaManager dbSchemaManager,
            IStructureSchemas structureSchemas,
            IJsonSerializer jsonSerializer)
            : base(connectionInfo, transactional, dbSchemaManager, structureSchemas, jsonSerializer)
        {
        }
    }
}