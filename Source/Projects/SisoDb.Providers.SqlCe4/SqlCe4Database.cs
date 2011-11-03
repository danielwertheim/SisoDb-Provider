using System;

namespace SisoDb.SqlCe4
{
    public class SqlCe4Database : SisoDatabase
    {
        protected internal SqlCe4Database(ISisoConnectionInfo connectionInfo)
            : base(connectionInfo)
        {
        }

        public override IQueryEngine CreateQueryEngine()
        {
            throw new NotImplementedException();
            //var jsonSerializer = SisoEnvironment.Resources.ResolveJsonSerializer();

            //return new SqlCe4QueryEngine(
            //    ConnectionInfo,
            //    DbSchemaManager,
            //    StructureSchemas,
            //    jsonSerializer);
        }

        public override IUnitOfWork CreateUnitOfWork()
        {
            throw new NotImplementedException();
            //var jsonSerializer = SisoEnvironment.Resources.ResolveJsonSerializer();

            //return new SqlCe4UnitOfWork(
            //    ConnectionInfo,
            //    DbSchemaManager,
            //    StructureSchemas,
            //    jsonSerializer,
            //    StructureBuilder);
        }
    }
}