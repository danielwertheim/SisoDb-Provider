using System;
using System.Data;
using System.Data.SqlClient;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Dac.BulkInserts;
using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.Querying;
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
            return new DbSchemaManager(new SqlDbSchemaUpserter(GetSqlStatements()));
        }

        public virtual ISqlStatements GetSqlStatements()
        {
            return _sqlStatements.Value;
        }

        public virtual IStructureInserter GetStructureInserter(IDbClient dbClient)
        {
            return new DbStructureInserter(dbClient);
        }

    	public IIdentityStructureIdGenerator GetIdentityStructureIdGenerator(IDbClient dbClient)
    	{
    		return new DbIdentityStructureIdGenerator(dbClient);
    	}

    	public virtual IDbQueryGenerator GetDbQueryGenerator()
        {
            return new Sql2008QueryGenerator(GetSqlStatements());
        }

    	public IQueryBuilder<T> GetQueryBuilder<T>(IStructureSchemas structureSchemas) where T : class
    	{
    		return new QueryBuilder<T>(structureSchemas, new ExpressionParsers());
    	}
    }
}