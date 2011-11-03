using System;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Dac.BulkInserts;
using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.Structures;

namespace SisoDb.SqlCe4
{
    public class SqlCe4ProviderFactory : ISisoProviderFactory
    {
        private readonly Lazy<ISqlStatements> _sqlStatements;

        public SqlCe4ProviderFactory()
        {
            _sqlStatements = new Lazy<ISqlStatements>(() => new SqlCe4Statements());
        }

        public virtual IServerClient GetServerClient(ISisoConnectionInfo connectionInfo)
        {
            throw new NotImplementedException();
            //return new SqlCe4ServerClient(connectionInfo);
        }

        public virtual IDbClient GetDbClient(ISisoConnectionInfo connectionInfo, bool transactional)
        {
            throw new NotImplementedException();
            //return new SqlCe4DbClient(connectionInfo, transactional);
        }

        public virtual IDbSchemaManager GetDbSchemaManager()
        {
            return new DbSchemaManager();
        }

        public virtual IDbSchemaUpserter GetDbSchemaUpserter(IDbClient dbClient)
        {
            return new SqlDbSchemaUpserter(dbClient);
        }

        public virtual ISqlStatements GetSqlStatements()
        {
            return _sqlStatements.Value;
        }

        public virtual IdentityStructureIdGenerator GetIdentityStructureIdGenerator(IDbClient dbClient)
        {
            return new IdentityStructureIdGenerator(dbClient);
        }

        public virtual IDbBulkInserter GetDbBulkInserter(IDbClient dbClient)
        {
            return new DbBulkInserter(dbClient);
        }

        public virtual IDbQueryGenerator GetDbQueryGenerator()
        {
            throw new NotImplementedException();
            //return new SqlCe4QueryGenerator(
            //    new LambdaToSqlWhereConverter(),
            //    new LambdaToSqlSortingConverter(),
            //    new LambdaToSqlIncludeConverter());
        }

        public virtual IGetCommandBuilder<T> CreateGetCommandBuilder<T>() where T : class
        {
            return new GetCommandBuilder<T>(
                new SortingParser(),
                new IncludeParser());
        }

        public virtual IQueryCommandBuilder<T> CreateQueryCommandBuilder<T>(IStructureSchema structureSchema) where T : class
        {
            return new QueryCommandBuilder<T>(
                structureSchema,
                new WhereParser(),
                new SortingParser(),
                new IncludeParser());
        }
    }
}