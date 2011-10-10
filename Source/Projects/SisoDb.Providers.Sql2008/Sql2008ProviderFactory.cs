using SisoDb.Dac;
using SisoDb.Dac.BulkInserts;
using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Processors.Sql;
using SisoDb.Querying.Sql;
using SisoDb.Sql2008.Dac;
using SisoDb.Sql2008.DbSchema;
using SisoDb.Structures;

namespace SisoDb.Sql2008
{
    public class Sql2008ProviderFactory : ISisoProviderFactory
    {
        public IServerClient GetServerClient(ISisoConnectionInfo connectionInfo)
        {
            return new Sql2008ServerClient(connectionInfo);
        }

        public IDbClient GetDbClient(ISisoConnectionInfo connectionInfo, bool transactional)
        {
            return new Sql2008DbClient(connectionInfo, transactional);
        }

        public IDbSchemaManager GetDbSchemaManager()
        {
            return new DbSchemaManager();
        }

        public IDbSchemaUpserter GetDbSchemaUpserter(IDbClient dbClient)
        {
            return new SqlDbSchemaUpserter(dbClient);
        }

        public IDbQueryGenerator GetDbQueryGenerator()
        {
            var memberPathGenerator = SisoEnvironment.Resources.ResolveMemberPathGenerator();

            return new Sql2008QueryGenerator(
                new ParsedWhereSqlProcessor(memberPathGenerator),
                new ParsedSortingSqlProcessor(memberPathGenerator),
                new ParsedIncludeSqlProcessor(memberPathGenerator));
        }

        public IDbBulkInserter GetDbBulkInserter(IDbClient dbClient)
        {
            return new Sql2008DbBulkInserter(dbClient);
        }

        public ICommandBuilderFactory GetCommandBuilderFactory()
        {
            return new CommandBuilderFactory();
        }

        public IdentityStructureIdGenerator GetIdentityStructureIdGenerator(IDbClient dbClient)
        {
            return new IdentityStructureIdGenerator(dbClient);
        }
    }
}