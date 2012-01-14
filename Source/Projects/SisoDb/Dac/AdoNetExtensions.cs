using System;
using System.Data;
using System.Linq;

namespace SisoDb.Dac
{
	public static class AdoNetExtensions
    {
		public static T GetScalarResult<T>(this IDbCommand cmd)
        {
            var value = cmd.ExecuteScalar();

            if (value == null || value == DBNull.Value)
                return default(T);

            return (T)Convert.ChangeType(value, typeof(T));
        }

		public static IDbCommand CreateCommand(this IDbConnection connection, string sql, params IDacParameter[] parameters)
        {
            return CreateCommand(connection, null, CommandType.Text, sql, parameters);
        }

		public static IDbCommand CreateCommand(this IDbConnection connection, IDbTransaction transaction, string sql, params IDacParameter[] parameters)
		{
			return CreateCommand(connection, transaction, CommandType.Text, sql, parameters);
		}

		public static IDbCommand CreateSpCommand(this IDbConnection connection, IDbTransaction transaction, string sql, params IDacParameter[] parameters)
		{
			return CreateCommand(connection, transaction, CommandType.StoredProcedure, sql, parameters);
		}

        private static IDbCommand CreateCommand(IDbConnection connection, IDbTransaction transaction, CommandType commandType, string sql, params IDacParameter[] parameters)
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

		public static void AddParameters(this IDbCommand cmd, params IDacParameter[] parameters)
        {
            foreach (var dacParam in parameters)
            {
                var parameter = cmd.CreateParameter();
                parameter.ParameterName = dacParam.Name;
                parameter.Value = dacParam.Value;

                cmd.Parameters.Add(parameter);
            }
        }

		public static void ExecuteNonQuery(this IDbConnection connection, string sql, params IDacParameter[] parameters)
        {
            using(var cmd = connection.CreateCommand(sql, parameters))
            {
                cmd.ExecuteNonQuery();
            }
        }

		public static void ExecuteNonQuery(this IDbConnection connection, IDbTransaction transaction, string sql, params IDacParameter[] parameters)
        {
            using (var cmd = connection.CreateCommand(transaction, sql, parameters))
            {
                cmd.ExecuteNonQuery();
            }
        }

		public static void ExecuteNonQuery(this IDbConnection connection, IDbTransaction transaction, string[] sqls, params IDacParameter[] parameters)
        {
            using (var cmd = connection.CreateCommand(transaction, string.Empty, parameters))
            {
                foreach (var sqlStatement in sqls.Where(statement => !string.IsNullOrWhiteSpace(statement))) 
                {
                    cmd.CommandText = sqlStatement.Trim();
                    cmd.ExecuteNonQuery();
                }
            }
        }

		public static T ExecuteScalarResult<T>(this IDbConnection connection, string sql, params IDacParameter[] parameters)
        {
            using (var cmd = connection.CreateCommand(sql, parameters))
            {
                return cmd.GetScalarResult<T>();
            }
        }
    }
}