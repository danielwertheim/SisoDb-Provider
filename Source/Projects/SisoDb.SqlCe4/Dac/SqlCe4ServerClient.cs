using System;
using System.Data;
using System.Data.SqlServerCe;
using EnsureThat;
using NCore;
using SisoDb.Core.Io;
using SisoDb.Dac;
using SisoDb.Resources;

namespace SisoDb.SqlCe4.Dac
{
    public class SqlCe4ServerClient : IServerClient
    {
        private readonly SqlCe4ConnectionInfo _connectionInfo;
    	private readonly IConnectionManager _connectionManager;
        private readonly ISqlStatements _sqlStatements;

		public SqlCe4ServerClient(SqlCe4ConnectionInfo connectionInfo, IConnectionManager connectionManager, ISqlStatements sqlStatements)
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

        public void EnsureNewDb()
        {
            DropDbIfItExists();
            CreateDbIfItDoesNotExist();
        }

        public void CreateDbIfItDoesNotExist()
        {
            if(DbExists())
                return;

            _connectionManager.ReleaseAllDbConnections();

            using (var engine = new SqlCeEngine(_connectionInfo.ClientConnectionString.PlainString))
            {
                engine.CreateDatabase();
            }

            InitializeExistingDb();
        }

        public void InitializeExistingDb()
        {
            if (!DbExists())
                throw new SisoDbException(ExceptionMessages.SqlDatabase_InitializeExisting_DbDoesNotExist.Inject(_connectionInfo.FilePath));

			_connectionManager.ReleaseAllDbConnections();

            WithConnection(cn =>
            {
                var exists = cn.ExecuteScalarResult<int>(_sqlStatements.GetSql("Sys_Identities_Exists")) > 0;

                if (exists)
                    return;

                cn.ExecuteNonQuery(_sqlStatements.GetSql("Sys_Identities_Create").Inject(_connectionInfo.DbName));
            });
        }

        public bool DbExists()
        {
            return IoHelper.FileExists(_connectionInfo.FilePath);
        }

        public void DropDbIfItExists()
        {
			_connectionManager.ReleaseAllDbConnections();

            IoHelper.DeleteIfFileExists(_connectionInfo.FilePath);
        }
    }
}