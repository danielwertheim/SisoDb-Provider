using System;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Dac.BulkInserts;
using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Converters.Sql;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.Querying.Sql;
using SisoDb.Sql2008.Dac;
using SisoDb.Structures;

namespace SisoDb.Sql2008
{
    public class Sql2008ProviderFactory : ISisoProviderFactory
    {
        private readonly Lazy<ISqlStatements> _sqlStatements;

        public Sql2008ProviderFactory()
        {
            _sqlStatements = new Lazy<ISqlStatements>(() => new Sql2008Statements());
        }

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

        public ISqlStatements GetSqlStatements()
        {
            return _sqlStatements.Value;
        }

        public IDbQueryGenerator GetDbQueryGenerator()
        {
            return new SqlQueryGenerator(
                new LambdaToSqlWhereConverter(),
                new LambdaToSqlSortingConverter(),
                new LambdaToSqlIncludeConverter());
        }

        public IDbBulkInserter GetDbBulkInserter(IDbClient dbClient)
        {
            return new DbBulkInserter(dbClient);
        }

        public IGetCommandBuilder<T> CreateGetCommandBuilder<T>() where T : class
        {
            return new GetCommandBuilder<T>(
                new SortingParser(),
                new IncludeParser());
        }

        public IQueryCommandBuilder<T> CreateQueryCommandBuilder<T>(IStructureSchema structureSchema) where T : class
        {
            return new QueryCommandBuilder<T>(
                structureSchema,
                new WhereParser(),
                new SortingParser(),
                new IncludeParser());
        }

        public IdentityStructureIdGenerator GetIdentityStructureIdGenerator(IDbClient dbClient)
        {
            return new IdentityStructureIdGenerator(dbClient);
        }
    }
}