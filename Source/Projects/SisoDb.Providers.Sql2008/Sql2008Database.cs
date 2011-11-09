namespace SisoDb.Sql2008
{
    public class Sql2008Database : SisoDatabase
    {
        protected internal Sql2008Database(ISisoConnectionInfo connectionInfo) : base(connectionInfo)
        {
        }

        public override IQueryEngine CreateQueryEngine()
        {
            var jsonSerializer = SisoEnvironment.Resources.ResolveJsonSerializer();

            return new Sql2008QueryEngine(
                ConnectionInfo,
                DbSchemaManager,
                StructureSchemas,
                jsonSerializer);
        }

        public override IUnitOfWork CreateUnitOfWork()
        {
            var jsonSerializer = SisoEnvironment.Resources.ResolveJsonSerializer();

            return new Sql2008UnitOfWork(
                ConnectionInfo,
                DbSchemaManager,
                StructureSchemas,
                jsonSerializer,
                StructureBuilder);
        }
    }
}