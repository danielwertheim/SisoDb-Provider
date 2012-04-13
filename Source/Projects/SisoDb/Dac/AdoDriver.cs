using System;
using System.Data;
using System.Data.SqlClient;
using EnsureThat;
using NCore;
using SisoDb.DbSchema;

namespace SisoDb.Dac
{
    public class AdoDriver : IAdoDriver
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
                parameter.Value = dacParameter.Value;

                cmd.Parameters.Add(OnBeforeAddParameter(parameter, dacParameter));
            }
        }

        protected virtual IDbDataParameter OnBeforeAddParameter(IDbDataParameter parameter, IDacParameter dacParameter)
        {
            if (DbSchemas.Parameters.IsSysParam(dacParameter))
            {
                parameter.DbType = DbType.AnsiString;
                parameter.Size = parameter.Value.ToString().Length;

                return parameter;
            }

            if(dacParameter.Value is string)
            {
                parameter.DbType = DbType.String;
                parameter.Size = (parameter.Value.ToStringOrNull() ?? " ").Length;

                return parameter;
            }

            if(dacParameter.Value is int)
            {
                parameter.DbType = DbType.Int32;
                return parameter;
            }

            if (dacParameter.Value is long)
            {
                parameter.DbType = DbType.Int64;
                return parameter;
            }

            if (dacParameter.Value is Guid)
            {
                parameter.DbType = DbType.Guid;
                return parameter;
            }

            if (dacParameter.Value is DateTime)
            {
                parameter.DbType = DbType.DateTime;
                return parameter;
            }

            if (dacParameter.Value is double || dacParameter.Value is decimal)
            {
                parameter.DbType = DbType.Decimal;
                return parameter;
            }

            if (dacParameter.Value is bool)
            {
                parameter.DbType = DbType.Boolean;
                return parameter;
            }

            return parameter;
        }
    }
}