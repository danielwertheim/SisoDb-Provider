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
            var jsonSerializer = SisoEnvironment.Resources.ResolveJsonSerializer();

            return new SqlCe4QueryEngine(
                ConnectionInfo,
                DbSchemaManager,
                StructureSchemas,
                jsonSerializer);
        }

        public override IUnitOfWork CreateUnitOfWork()
        {
            var jsonSerializer = SisoEnvironment.Resources.ResolveJsonSerializer();

            return new SqlCe4UnitOfWork(
                ConnectionInfo,
                DbSchemaManager,
                StructureSchemas,
                jsonSerializer,
                StructureBuilder);
        }
    }
}