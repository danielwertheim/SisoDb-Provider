using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Dac.BulkInserts;
using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.SqlCe4.Dac;
using SisoDb.Structures;

namespace SisoDb.SqlCe4
{
    public class SqlCe4ProviderFactory : ISisoProviderFactory
    {
        private readonly Lazy<ISqlStatements> _sqlStatements;

        private readonly ConcurrentDictionary<string, BlockingCollection<IDbConnection>> _clientConnections;

        public SqlCe4ProviderFactory()
        {
            _sqlStatements = new Lazy<ISqlStatements>(() => new SqlCe4Statements());
            _clientConnections = new ConcurrentDictionary<string, BlockingCollection<IDbConnection>>();
        }

        ~SqlCe4ProviderFactory()
        {
            ReleaseAllClientConnections();
        }

        public StorageProviders ProviderType
        {
            get { return StorageProviders.SqlCe4; }
        }

        public IDbConnection GetOpenServerConnection(IConnectionString connectionString)
        {
            var cn = new SqlCeConnection(connectionString.PlainString);
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
            IDbConnection cn;

            var connectionsPerCnString = GetConnectionsForConnectionString(connectionString.PlainString);
            if (connectionsPerCnString.TryTake(out cn))
                return cn;

            cn = new SqlCeConnection(connectionString.PlainString);
            cn.Open();
            return cn;
        }

        public void ReleaseConnection(IDbConnection dbConnection)
        {
            var connectionsPerCnString = GetConnectionsForConnectionString(dbConnection.ConnectionString);
            if(!connectionsPerCnString.TryAdd(dbConnection))
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        private BlockingCollection<IDbConnection> GetConnectionsForConnectionString(string connectionString)
        {
            return _clientConnections.GetOrAdd(connectionString, new BlockingCollection<IDbConnection>());
        }

        public void ReleaseAllClientConnections()
        {
            var exceptions = new List<Exception>();

            foreach (var key in _clientConnections.Keys)
            {
                try
                {
                    BlockingCollection<IDbConnection> connections;
                    if (_clientConnections.TryRemove(key, out connections))
                    {
                        while(connections.Count > 0)
                        {
                            try
                            {
                                IDbConnection cn;
                                if (connections.TryTake(out cn) && cn != null)
                                {
                                    if (cn.State != ConnectionState.Closed)
                                        cn.Close();

                                    cn.Dispose();
                                }   
                            }
                            catch (Exception ex)
                            {
                                exceptions.Add(ex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            _clientConnections.Clear();

            if (exceptions.Count > 0)
                throw new SisoDbException("Exceptions occured while releasing SqlCe4Connections from the pool.", exceptions);
        }

        public virtual IServerClient GetServerClient(ISisoConnectionInfo connectionInfo)
        {
            return new SqlCe4ServerClient((SqlCe4ConnectionInfo)connectionInfo);
        }

        public IDbClient GetTransactionalDbClient(ISisoConnectionInfo connectionInfo)
        {
            return new SqlCe4DbClient(connectionInfo, true);
        }

        public IDbClient GetNonTransactionalDbClient(ISisoConnectionInfo connectionInfo)
        {
            return new SqlCe4DbClient(connectionInfo, false);
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
            return new SqlCe4QueryGenerator(GetSqlStatements());
        }

    	public IQueryBuilder<T> GetQueryBuilder<T>(IStructureSchemas structureSchemas) where T : class
    	{
    		return new QueryBuilder<T>(structureSchemas, new ExpressionParsers());
    	}

    	public IExpressionParsers GetExpressionParsers()
		{
			return new ExpressionParsers();
		}
    }
}