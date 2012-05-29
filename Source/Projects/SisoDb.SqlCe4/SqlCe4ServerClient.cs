using System.Data.SqlServerCe;
using NCore;
using SisoDb.Core.Io;
using SisoDb.Dac;
using SisoDb.Resources;

namespace SisoDb.SqlCe4
{
    public class SqlCe4ServerClient : DbServerClient
    {
        private readonly SqlCe4ConnectionInfo _connectionInfo;
        
        public SqlCe4ServerClient(IAdoDriver driver, SqlCe4ConnectionInfo connectionInfo, IConnectionManager connectionManager, ISqlStatements sqlStatements) 
            : base(driver, connectionInfo, connectionManager, sqlStatements)
        {
            _connectionInfo = connectionInfo;
        }

        public override void EnsureNewDb()
        {
            DropDbIfItExists();
            CreateDbIfItDoesNotExist();
        }

        public override void CreateDbIfItDoesNotExist()
        {
            if(DbExists())
                return;

            ConnectionManager.ReleaseAllDbConnections();

            using (var engine = new SqlCeEngine(_connectionInfo.ClientConnectionString.PlainString))
            {
                engine.CreateDatabase();
            }

            InitializeExistingDb();
        }

        public override void InitializeExistingDb()
        {
            if (!DbExists())
                throw new SisoDbException(ExceptionMessages.SqlDatabase_InitializeExisting_DbDoesNotExist.Inject(_connectionInfo.FilePath));

			ConnectionManager.ReleaseAllDbConnections();

            WithConnection(cn =>
            {
                var exists = OnExecuteScalar<int>(cn, SqlStatements.GetSql("Sys_Identities_Exists")) > 0;

                if (exists)
                    return;

                OnExecuteNonQuery(cn, SqlStatements.GetSql("Sys_Identities_Create").Inject(_connectionInfo.DbName));
            });
        }

        public override bool DbExists()
        {
            return IoHelper.FileExists(_connectionInfo.FilePath);
        }

        public override void DropDbIfItExists()
        {
			ConnectionManager.ReleaseAllDbConnections();

            IoHelper.DeleteIfFileExists(_connectionInfo.FilePath);
        }
    }
}