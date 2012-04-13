using System.Data;
using System.Data.SqlServerCe;
using EnsureThat;
using SisoDb.Dac;

namespace SisoDb.SqlCe4.Dac
{
    public class SqlCe4AdoDriver : AdoDriver
    {
        public override IDbConnection CreateConnection(string connectionString)
        {
            Ensure.That(connectionString, "connectionString").IsNotNull();

            return new SqlCeConnection(connectionString);
        }

        protected override IDbDataParameter OnBeforeAddParameter(IDbDataParameter parameter, IDacParameter dacParameter)
        {
            return parameter;
        }
    }
}