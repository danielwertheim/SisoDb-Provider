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
            return new SqlCe4QueryEngine(this, DbSchemaManager);
        }

        public override IUnitOfWork CreateUnitOfWork()
        {
            return new SqlCe4UnitOfWork(this, DbSchemaManager);
        }
    }
}