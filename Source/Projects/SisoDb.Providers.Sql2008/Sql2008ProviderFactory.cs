using System;
using System.Data;
using System.Data.SqlClient;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Dac.BulkInserts;
using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Converters.Sql;
using SisoDb.Querying.Lambdas.Parsers;
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

        public StorageProviders ProviderType
        {
            get { return StorageProviders.Sql2008; }
        }

        public IDbConnection GetOpenServerConnection(IConnectionString connectionString)
        {
            var cn = new SqlConnection(connectionString.PlainString);
            cn.Open();

            return cn;
        }

        public void ReleaseServerConnection(IDbConnection dbConnection)
        {
            if (dbConnection == null)
                return;

            if (dbConnection.State != ConnectionState.Closed)
                dbConnection.Close();

            dbConnection.Dispose();
        }

        public IDbConnection GetOpenConnection(IConnectionString connectionString)
        {
            var cn = new SqlConnection(connectionString.PlainString);
            cn.Open();

            return cn;
        }

        public void ReleaseConnection(IDbConnection dbConnection)
        {
            if(dbConnection == null)
                return;

            if (dbConnection.State != ConnectionState.Closed)
                dbConnection.Close();
            
            dbConnection.Dispose();
        }

        public virtual IServerClient GetServerClient(ISisoConnectionInfo connectionInfo)
        {
            return new Sql2008ServerClient((Sql2008ConnectionInfo)connectionInfo);
        }

        public IDbClient GetTransactionalDbClient(ISisoConnectionInfo connectionInfo)
        {
            return new Sql2008DbClient(connectionInfo, true);
        }

        public IDbClient GetNonTransactionalDbClient(ISisoConnectionInfo connectionInfo)
        {
            return new Sql2008DbClient(connectionInfo, false);
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

        public virtual IDbStructureInserter GetDbStructureInserter(IDbClient dbClient)
        {
            return new DbStructureInserter(dbClient);
        }

        public virtual IDbQueryGenerator GetDbQueryGenerator()
        {
            return new Sql2008QueryGenerator(
                new LambdaToSqlWhereConverter(),
                new LambdaToSqlSortingConverter(),
                new LambdaToSqlIncludeConverter());
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