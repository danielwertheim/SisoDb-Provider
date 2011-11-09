using PineCone.Structures.Schemas;
using SisoDb.DbSchema;
using SisoDb.Serialization;

namespace SisoDb.SqlCe4
{
    public class SqlCe4QueryEngine : DbQueryEngine
    {
        internal SqlCe4QueryEngine(
            ISisoConnectionInfo connectionInfo,
            IDbSchemaManager dbSchemaManager,
            IStructureSchemas structureSchemas,
            IJsonSerializer jsonSerializer) 
            : base(connectionInfo, false, dbSchemaManager, structureSchemas, jsonSerializer)
        {}

        protected SqlCe4QueryEngine(
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