using System;
using System.Data;
using EnsureThat;
using NCore;
using SisoDb.Dac;
using SisoDb.Providers;
using SisoDb.Resources;

namespace SisoDb.Sql2008.Dac
{
    public class Sql2008ServerClient : IServerClient
    {
        private readonly Sql2008ConnectionInfo _connectionInfo;
        private readonly ISisoProviderFactory _providerFactory;
        private readonly ISqlStatements _sqlStatements;

        public Sql2008ServerClient(Sql2008ConnectionInfo connectionInfo)
        {
            Ensure.That(connectionInfo, "connectionInfo").IsNotNull();

            _connectionInfo = connectionInfo;
            _providerFactory = SisoEnvironment.ProviderFactories.Get(_connectionInfo.ProviderType);
            _sqlStatements = _providerFactory.GetSqlStatements();
        }

        private void WithConnection(Action<IDbConnection> cnConsumer)
        {
            IDbConnection cn = null;

            try
            {
                cn = _providerFactory.GetOpenServerConnection(_connectionInfo.ServerConnectionString);
                cnConsumer.Invoke(cn);
            }
            finally
            {
                _providerFactory.ReleaseServerConnection(cn);
            }
        }

        private T WithConnection<T>(Func<IDbConnection, T> cnConsumer)
        {
            T result;
            IDbConnection cn = null;

            try
            {
                cn = _providerFactory.GetOpenConnection(_connectionInfo.ServerConnectionString);
                result = cnConsumer.Invoke(cn);
            }
            finally
            {
                _providerFactory.ReleaseConnection(cn);
            }

            return result;
        }

        public void EnsureNewDb()
        {
            WithConnection(cn =>
            {
                cn.ExecuteNonQuery(_sqlStatements.GetSql("DropDatabase").Inject(_connectionInfo.DbName));
                cn.ExecuteNonQuery(_sqlStatements.GetSql("CreateDatabase").Inject(_connectionInfo.DbName));
                cn.ExecuteNonQuery(_sqlStatements.GetSql("Sys_Identities_CreateIfNotExists").Inject(_connectionInfo.DbName));
                cn.ExecuteNonQuery(_sqlStatements.GetSql("Sys_Types_CreateIfNotExists").Inject(_connectionInfo.DbName));
            });
        }

        public void CreateDbIfDoesNotExists()
        {
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
            WithConnection(cn => cn.ExecuteNonQuery(_sqlStatements.GetSql("DropDatabase").Inject(_connectionInfo.DbName)));
        }
    }
}