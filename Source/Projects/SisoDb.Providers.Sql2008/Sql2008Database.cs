using SisoDb.Structures;

namespace SisoDb.Sql2008
{
    public class Sql2008Database : DbDatabase
    {
        protected internal Sql2008Database(ISisoConnectionInfo connectionInfo) : base(connectionInfo)
        {
        }

		public override IQueryEngine CreateQueryEngine()
        {
            return new Sql2008QueryEngine(
				this,
				ProviderFactory.GetNonTransactionalDbClient(ConnectionInfo),
				DbSchemaManager);
        }

        public override IUnitOfWork CreateUnitOfWork()
        {
        	var dbClient = ProviderFactory.GetTransactionalDbClient(ConnectionInfo);
			var dbClientNonTransactional = ProviderFactory.GetNonTransactionalDbClient(ConnectionInfo);

			return new Sql2008UnitOfWork(
				this,
				dbClient,
				dbClientNonTransactional,
				DbSchemaManager,
				ProviderFactory.GetIdentityStructureIdGenerator(dbClientNonTransactional));
        }
    }
}