using System.Data;
using System.Data.SqlClient;
using NCore;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.SqlServer;

namespace SisoDb.Sql2005
{
    public class Sql2005AdoDriver : SqlServerAdoDriver
    {
        protected override IDbDataParameter OnParameterCreated(IDbDataParameter parameter, IDacParameter dacParameter)
        {
            var dbParam = (SqlParameter)parameter;
            var setSize = false;

            if (DbSchemas.Parameters.ShouldBeMultivalue(dacParameter))
            {
                var arrayDacParam = (ArrayDacParameter)dacParameter;
                return SqlServerTableParams.Create(
                    arrayDacParam.Name,
                    arrayDacParam.MemberDataType,
                    arrayDacParam.MemberDataTypeCode,
                    (object[])dacParameter.Value);
            }

            if (DbSchemas.Parameters.ShouldBeDateTime(dacParameter))
            {
                dbParam.DbType = DbType.DateTime;
                return dbParam;
            }

            if (DbSchemas.Parameters.ShouldBeNonUnicodeString(dacParameter))
            {
                dbParam.SqlDbType = SqlDbType.VarChar;
                setSize = true;
            }
            else if (DbSchemas.Parameters.ShouldBeUnicodeString(dacParameter))
            {
                dbParam.SqlDbType = SqlDbType.NVarChar;
                setSize = true;
            }

            if (setSize)
            {
                dbParam.Size = (dacParameter.Value.ToStringOrNull() ?? string.Empty).Length;
                return dbParam;
            }

            return dbParam;
        }
    }
}