using System;
using System.Data;
using System.Data.SqlServerCe;
using EnsureThat;
using NCore;
using SisoDb.Core.Io;
using SisoDb.Dac;
using SisoDb.Providers;
using SisoDb.Resources;

namespace SisoDb.SqlCe4.Dac
{
    public class SqlCe4ServerClient : IServerClient
    {
        private readonly SqlCe4ConnectionInfo _connectionInfo;
        private readonly SqlCe4ProviderFactory _providerFactory;
        private readonly ISqlStatements _sqlStatements;

        public SqlCe4ServerClient(SqlCe4ConnectionInfo connectionInfo)
        {
            Ensure.That(connectionInfo, "connectionInfo").IsNotNull();

            _connectionInfo = connectionInfo;
            _providerFactory = (SqlCe4ProviderFactory)SisoEnvironment.ProviderFactories.Get(_connectionInfo.ProviderType);
            _sqlStatements = _providerFactory.GetSqlStatements();
        }

        private void WithConnection(Action<IDbConnection> cnConsumer)
        {
            IDbConnection cn = null;

            try
            {
                cn = _providerFactory.GetOpenServerConnection(_connectionInfo.ConnectionString);
                cnConsumer.Invoke(cn);
            }
            finally
            {
                _providerFactory.ReleaseServerConnection(cn);
            }
        }

        public void EnsureNewDb()
        {
            DropDbIfItExists();
            CreateDbIfDoesNotExists();
        }

        public void CreateDbIfDoesNotExists()
        {
            if(DbExists())
                return;

            using (var engine = new SqlCeEngine(_connectionInfo.ConnectionString.PlainString))
            {
                engine.CreateDatabase();
            }

            InitializeExistingDb();
        }

        public void InitializeExistingDb()
        {
            if (!DbExists())
                throw new SisoDbException(ExceptionMessages.SqlDatabase_InitializeExisting_DbDoesNotExist.Inject(_connectionInfo.FilePath));

            _providerFactory.ReleaseAllClientConnections();
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
            _providerFactory.ReleaseAllClientConnections();

            IoHelper.DeleteIfFileExists(_connectionInfo.FilePath);
        }
    }
}