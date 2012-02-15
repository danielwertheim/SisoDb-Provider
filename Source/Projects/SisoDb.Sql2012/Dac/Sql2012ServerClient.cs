using System;
using System.Data;
using EnsureThat;
using NCore;
using SisoDb.Dac;
using SisoDb.Resources;

namespace SisoDb.Sql2012.Dac
{
    public class Sql2012ServerClient : IServerClient
    {
        private readonly ISisoConnectionInfo _connectionInfo;
    	private readonly IConnectionManager _connectionManager;
        private readonly ISqlStatements _sqlStatements;

		public Sql2012ServerClient(ISisoConnectionInfo connectionInfo, IConnectionManager connectionManager, ISqlStatements sqlStatements)
        {
            Ensure.That(connectionInfo, "connectionInfo").IsNotNull();
			Ensure.That(connectionManager, "connectionManager").IsNotNull();
			Ensure.That(sqlStatements, "sqlStatements").IsNotNull();

            _connectionInfo = connectionInfo;
        	_connectionManager = connectionManager;
            _sqlStatements = sqlStatements;
        }

        private void WithConnection(Action<IDbConnection> cnConsumer)
        {
            IDbConnection cn = null;

            try
            {
				cn = _connectionManager.OpenServerConnection(_connectionInfo);
                cnConsumer.Invoke(cn);
            }
            finally
            {
				_connectionManager.ReleaseServerConnection(cn);
            }
        }

        private T WithConnection<T>(Func<IDbConnection, T> cnConsumer)
        {
            T result;
            IDbConnection cn = null;

            try
            {
				cn = _connectionManager.OpenServerConnection(_connectionInfo);
                result = cnConsumer.Invoke(cn);
            }
            finally
            {
				_connectionManager.ReleaseServerConnection(cn);
            }

            return result;
        }

        public void EnsureNewDb()
        {
			_connectionManager.ReleaseAllDbConnections();

            WithConnection(cn =>
            {
                cn.ExecuteNonQuery(_sqlStatements.GetSql("DropDatabase").Inject(_connectionInfo.DbName));
                cn.ExecuteNonQuery(_sqlStatements.GetSql("CreateDatabase").Inject(_connectionInfo.DbName));
                cn.ExecuteNonQuery(_sqlStatements.GetSql("Sys_Identities_CreateIfNotExists").Inject(_connectionInfo.DbName));
                cn.ExecuteNonQuery(_sqlStatements.GetSql("Sys_Types_CreateIfNotExists").Inject(_connectionInfo.DbName));
            });
        }

        public void CreateDbIfItDoesNotExist()
        {
			_connectionManager.ReleaseAllDbConnections();

            WithConnection(cn =>
            {
                var exists = cn.ExecuteScalarResult<int>(_sqlStatements.GetSql("DatabaseExists"), new DacParameter("dbName", _connectionInfo.DbName)) > 0;

                if(exists)
                    return;
                
                cn.ExecuteNonQuery(_sqlStatements.GetSql("CreateDatabase").Inject(_connectionInfo.DbName));
                cn.ExecuteNonQuery(_sqlStatements.GetSql("Sys_Identities_CreateIfNotExists").Inject(_connectionInfo.DbName));
                cn.ExecuteNonQuery(_sqlStatements.GetSql("Sys_Types_CreateIfNotExists").Inject(_connectionInfo.DbName));
            });
        }

        public void InitializeExistingDb()
        {
			_connectionManager.ReleaseAllDbConnections();

            WithConnection(cn =>
            {
                var exists = cn.ExecuteScalarResult<int>(_sqlStatements.GetSql("DatabaseExists"), new DacParameter("dbName", _connectionInfo.DbName)) > 0;

                if (!exists)
                    throw new SisoDbException(ExceptionMessages.SqlDatabase_InitializeExisting_DbDoesNotExist.Inject(_connectionInfo.DbName));

                cn.ExecuteNonQuery(_sqlStatements.GetSql("Sys_Identities_CreateIfNotExists").Inject(_connectionInfo.DbName));
                cn.ExecuteNonQuery(_sqlStatements.GetSql("Sys_Types_CreateIfNotExists").Inject(_connectionInfo.DbName));
            });
        }

        public bool DbExists()
        {
            return WithConnection(cn => cn.ExecuteScalarResult<int>(_sqlStatements.GetSql("DatabaseExists"), new DacParameter("dbName", _connectionInfo.DbName)) > 0);
        }

        public void DropDbIfItExists()
        {
			_connectionManager.ReleaseAllDbConnections();

            WithConnection(cn => cn.ExecuteNonQuery(_sqlStatements.GetSql("DropDatabase").Inject(_connectionInfo.DbName)));
        }
    }
}