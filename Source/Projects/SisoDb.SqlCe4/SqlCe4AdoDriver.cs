using System.Data;
using System.Data.SqlServerCe;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.EnsureThat;
using SisoDb.NCore;
using SisoDb.Resources;
using SisoDb.SqlServer;

namespace SisoDb.SqlCe4
{
    public class SqlCe4AdoDriver : SqlServerAdoDriver
    {
        public const int MaxLenOfStringBeforeEscalating = 4000;

        public SqlCe4AdoDriver()
        {
            CommandTimeout = 0;
        }

        public override IDbConnection CreateConnection(string connectionString)
        {
            Ensure.That(connectionString, "connectionString").IsNotNull();

            return new SqlCeConnection(connectionString);
        }

        protected override IDbDataParameter OnParameterCreated(IDbDataParameter parameter, IDacParameter dacParameter)
        {
            var dbParam = (SqlCeParameter)parameter;

            if (DbSchemaInfo.Parameters.ShouldBeDateTime(dacParameter))
            {
                dbParam.DbType = DbType.DateTime;
                return dbParam;
            }

            if (DbSchemaInfo.Parameters.ShouldBeJson(dacParameter))
            {
                dbParam.SqlDbType = SqlDbType.NText;
                dbParam.Size = (dacParameter.Value.ToStringOrNull() ?? string.Empty).Length;
                return dbParam;
            }

            if (DbSchemaInfo.Parameters.ShouldBeUnicodeString(dacParameter))
            {
                dbParam.SqlDbType = SqlDbType.NVarChar;
                var len = (dacParameter.Value.ToStringOrNull() ?? string.Empty).Length;
                if (len > MaxLenOfStringBeforeEscalating)
                    throw new SisoDbException(ExceptionMessages.SqlCe4_ToLongIndividualStringValue);

                dbParam.Size = len;
                return dbParam;
            }

            return dbParam;
        }
    }
}