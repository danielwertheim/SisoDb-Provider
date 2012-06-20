using System.Data;
using SisoDb.Dac;
using SisoDb.SqlServer;

namespace SisoDb.Sql2005
{
    public class Sql2005ServerClient : SqlServerClient
    {
        public Sql2005ServerClient(IAdoDriver driver, ISisoConnectionInfo connectionInfo, IConnectionManager connectionManager, ISqlStatements sqlStatements) 
            : base(driver, connectionInfo, connectionManager, sqlStatements) {}

        protected override void OnInitializeSysTypes(IDbConnection cn)
        {
        }
    }
}