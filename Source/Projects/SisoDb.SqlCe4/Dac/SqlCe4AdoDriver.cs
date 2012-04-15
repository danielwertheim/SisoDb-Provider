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

        protected override IDbDataParameter OnParameterCreated(IDbDataParameter parameter, IDacParameter dacParameter)
        {
            parameter = base.OnParameterCreated(parameter, dacParameter);

            if(parameter.DbType == DbType.AnsiString)
                parameter.DbType = DbType.String;

            return parameter;
        }
    }
}