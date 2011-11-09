using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.DbSchema;
using SisoDb.Serialization;

namespace SisoDb.SqlCe4
{
    public class SqlCe4UnitOfWork : DbUnitOfWork
    {
        protected internal SqlCe4UnitOfWork(
            ISisoConnectionInfo connectionInfo,
            IDbSchemaManager dbSchemaManager,
            IStructureSchemas structureSchemas,
            IJsonSerializer jsonSerializer,
            IStructureBuilder structureBuilder)
            : base(connectionInfo, dbSchemaManager, structureSchemas, jsonSerializer, structureBuilder)
        {
        }
    }
}