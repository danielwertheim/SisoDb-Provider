namespace SisoDb.Sql2008
{
    public class Sql2008Database : SisoDatabase
    {
        protected internal Sql2008Database(ISisoConnectionInfo connectionInfo) : base(connectionInfo)
        {
        }

        public override IQueryEngine CreateQueryEngine()
        {
            return new Sql2008QueryEngine(this, DbSchemaManager);
        }

        public override IUnitOfWork CreateUnitOfWork()
        {
            return new Sql2008UnitOfWork(this, DbSchemaManager);
        }
    }
}