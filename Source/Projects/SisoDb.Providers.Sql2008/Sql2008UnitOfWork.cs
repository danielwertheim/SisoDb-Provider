using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.DbSchema;
using SisoDb.Serialization;

namespace SisoDb.Sql2008
{
    public class Sql2008UnitOfWork : DbUnitOfWork
    {
        protected internal Sql2008UnitOfWork(
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