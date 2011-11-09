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
        private readonly ISisoProviderFactory _providerFactory;
        private readonly ISqlStatements _sqlStatements;

        public SqlCe4ServerClient(SqlCe4ConnectionInfo connectionInfo)
        {
            Ensure.That(connectionInfo, "connectionInfo").IsNotNull();

            _connectionInfo = connectionInfo;
            _providerFactory = SisoEnvironment.ProviderFactories.Get(_connectionInfo.ProviderType);
            _sqlStatements = _providerFactory.GetSqlStatements();
        }

        private void WithConnection(Action<SqlCeConnection> cnConsumer)
        {
            using (var cn = new SqlCeConnection(_connectionInfo.ConnectionString.PlainString))
            {
                cn.Open();

                cnConsumer.Invoke(cn);

                if (cn.State == ConnectionState.Open)
                    cn.Close();
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
            IoHelper.DeleteIfFileExists(_connectionInfo.FilePath);
        }
    }
}