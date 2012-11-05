using System.Data.SqlServerCe;
using SisoDb.Dac;
using SisoDb.NCore;
using SisoDb.NCore.Io;
using SisoDb.Resources;
using SisoDb.SqlServer;

namespace SisoDb.SqlCe4
{
    public class SqlCe4ServerClient : SqlServerClient
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

            ConnectionManager.ReleaseAllConnections();

            using (var engine = new SqlCeEngine(_connectionInfo.ClientConnectionString))
            {
                engine.CreateDatabase();
            }

            InitializeExistingDb();
        }

        public override void InitializeExistingDb()
        {
            if (!DbExists())
                throw new SisoDbException(ExceptionMessages.SqlDatabase_InitializeExisting_DbDoesNotExist.Inject(_connectionInfo.FilePath));

			ConnectionManager.ReleaseAllConnections();

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
            return IoUtils.FileExists(_connectionInfo.FilePath);
        }

        public override void DropDbIfItExists()
        {
			ConnectionManager.ReleaseAllConnections();

            IoUtils.DeleteIfFileExists(_connectionInfo.FilePath);
        }
    }
}