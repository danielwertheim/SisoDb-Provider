using System.Data;
using System.Data.SqlServerCe;
using EnsureThat;
using NCore;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Resources;

namespace SisoDb.SqlCe4.Dac
{
    public class SqlCe4AdoDriver : SqlDbAdoDriver
    {
        private const int MaxLenOfStringBeforeEscalating = 4000;

        public override IDbConnection CreateConnection(string connectionString)
        {
            Ensure.That(connectionString, "connectionString").IsNotNull();

            return new SqlCeConnection(connectionString);
        }

        protected override IDbDataParameter OnParameterCreated(IDbDataParameter parameter, IDacParameter dacParameter)
        {
            var dbParam = (SqlCeParameter)parameter;

            if (DbSchemas.Parameters.ShouldBeUnicodeString(dacParameter))
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