using System.Data;
using System.Data.SqlServerCe;
using EnsureThat;
using SisoDb.Dac;
using SisoDb.DbSchema;

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
            if (DbSchemas.Parameters.IsSysParam(dacParameter))
            {
                parameter.DbType = DbType.String;
                parameter.Size = dacParameter.Value.ToString().Length;

                return parameter;
            }

            return base.OnParameterCreated(parameter, dacParameter);
        }
    }
}