using System;
using System.Data;
using System.Linq;

namespace SisoDb.Dac
{
    internal static class AdoNetExtensions
    {
        internal static T GetScalarResult<T>(this IDbCommand cmd)
        {
            var value = cmd.ExecuteScalar();

            if (value == null || value == DBNull.Value)
                return default(T);

            return (T)Convert.ChangeType(value, typeof(T));
        }

        internal static IDbCommand CreateCommand(this IDbConnection connection, CommandType commandType, string sql, params IDacParameter[] parameters)
        {
            return connection.CreateCommand(null, commandType, sql, parameters);
        }

        internal static IDbCommand CreateCommand(this IDbConnection connection, IDbTransaction transaction, CommandType commandType, string sql, params IDacParameter[] parameters)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandType = commandType;
            cmd.UpdatedRowSource = UpdateRowSource.None;

            if (!string.IsNullOrWhiteSpace(sql))
                cmd.CommandText = sql;

            if (transaction != null)
                cmd.Transaction = transaction;

            cmd.AddParameters(parameters);
            
            return cmd;
        }

        internal static void AddParameters(this IDbCommand cmd, params IDacParameter[] parameters)
        {
            foreach (var dacParam in parameters)
            {
                var parameter = cmd.CreateParameter();
                parameter.ParameterName = dacParam.Name;
                parameter.Value = dacParam.Value;

                cmd.Parameters.Add(parameter);
            }
        }

        internal static void ExecuteNonQuery(this IDbConnection connection, string sql, params IDacParameter[] parameters)
        {
            using(var cmd = connection.CreateCommand(CommandType.Text, sql, parameters))
            {
                cmd.ExecuteNonQuery();
            }
        }

        internal static void ExecuteNonQuery(this IDbConnection connection, IDbTransaction transaction, string sql, params IDacParameter[] parameters)
        {
            using (var cmd = connection.CreateCommand(transaction, CommandType.Text, sql, parameters))
            {
                cmd.ExecuteNonQuery();
            }
        }

        internal static void ExecuteNonQuery(this IDbConnection connection, IDbTransaction transaction, string[] sqls, params IDacParameter[] parameters)
        {
            using (var cmd = connection.CreateCommand(transaction, CommandType.Text, string.Empty, parameters))
            {
                foreach (var sqlStatement in sqls.Where(statement => !string.IsNullOrWhiteSpace(statement))) 
                {
                    cmd.CommandText = sqlStatement.Trim();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        internal static T ExecuteScalarResult<T>(this IDbConnection connection, string sql, params IDacParameter[] parameters)
        {
            using (var cmd = connection.CreateCommand(CommandType.Text, sql, parameters))
            {
                return cmd.GetScalarResult<T>();
            }
        }
    }
}