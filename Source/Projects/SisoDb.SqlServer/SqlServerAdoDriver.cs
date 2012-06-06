using System.Data;
using System.Data.SqlClient;
using EnsureThat;
using NCore;
using SisoDb.Dac;
using SisoDb.DbSchema;

namespace SisoDb.SqlServer
{
    public class SqlServerAdoDriver : IAdoDriver
    {
        public virtual IDbConnection CreateConnection(string connectionString)
        {
            Ensure.That(connectionString, "connectionString").IsNotNull();

            return new SqlConnection(connectionString);
        }

        public virtual IDbCommand CreateCommand(IDbConnection connection, string sql, IDbTransaction transaction = null, params IDacParameter[] parameters)
        {
            return CreateCommand(connection, CommandType.Text, sql, transaction, parameters);
        }

        public virtual IDbCommand CreateSpCommand(IDbConnection connection, string spName, IDbTransaction transaction = null, params IDacParameter[] parameters)
        {
            return CreateCommand(connection, CommandType.StoredProcedure, spName, transaction, parameters);
        }

        protected virtual IDbCommand CreateCommand(IDbConnection connection, CommandType commandType, string sql, IDbTransaction transaction = null, IDacParameter[] parameters = null)
        {
            var cmd = connection.CreateCommand();
            if (transaction != null)
                cmd.Transaction = transaction;

            cmd.CommandType = commandType;
            cmd.UpdatedRowSource = UpdateRowSource.None;

            if (!string.IsNullOrWhiteSpace(sql))
                cmd.CommandText = sql;

            AddCommandParametersTo(cmd, parameters);

            return cmd;
        }

        public virtual void AddCommandParametersTo(IDbCommand cmd, params IDacParameter[] parameters)
        {
            foreach (var dacParameter in parameters)
            {
                var parameter = cmd.CreateParameter();
                parameter.ParameterName = dacParameter.Name;

                parameter = OnParameterCreated(parameter, dacParameter);

                if(parameter.Value == null)
                    parameter.Value = dacParameter.Value; //PERF: Yes, value should be set after OnParameterCreated otherwise ADO.Net will do some type mapping before we do.

                cmd.Parameters.Add(parameter);
            }
        }

        protected virtual IDbDataParameter OnParameterCreated(IDbDataParameter parameter, IDacParameter dacParameter)
        {
            var dbParam = (SqlParameter)parameter;
            var setSize = false;

            if (DbSchemas.Parameters.ShouldBeMultivalue(dacParameter))
                return SqlServerTableParams.Create(dbParam.ParameterName, (object[]) dacParameter.Value);

            if (DbSchemas.Parameters.ShouldBeDateTime(dacParameter))
            {
                dbParam.DbType = DbType.DateTime2;
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

            if(setSize)
            {
                dbParam.Size = (dacParameter.Value.ToStringOrNull() ?? string.Empty).Length;
                return dbParam;
            }

            return dbParam;
        }
    }
}