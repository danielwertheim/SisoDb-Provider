using System;
using System.Data;
using System.Linq;
using NCore;

namespace SisoDb.Dac
{
	public static class AdoNetExtensions
    {
		public static IDbCommand CreateCommand(this IDbConnection connection, IDbTransaction transaction = null)
		{
			return CreateCommand(connection, CommandType.Text, null, transaction);
		}

        public static IDbCommand CreateCommand(this IDbConnection connection, string sql, IDbTransaction transaction = null, params IDacParameter[] parameters)
        {
            return CreateCommand(connection, CommandType.Text, sql, transaction, parameters);
        }

        public static IDbCommand CreateSpCommand(this IDbConnection connection, string sql, IDbTransaction transaction = null, params IDacParameter[] parameters)
		{
			return CreateCommand(connection, CommandType.StoredProcedure, sql, transaction, parameters);
		}

        private static IDbCommand CreateCommand(IDbConnection connection, CommandType commandType, string sql, IDbTransaction transaction = null, params IDacParameter[] parameters)
        {
            var cmd = connection.CreateCommand();
            if(transaction != null)
                cmd.Transaction = transaction;

            cmd.CommandType = commandType;
            cmd.UpdatedRowSource = UpdateRowSource.None;

            if (!string.IsNullOrWhiteSpace(sql))
                cmd.CommandText = sql;

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

                if (ParamIsAnsiString(dacParam))
                {
                    parameter.DbType = DbType.AnsiString;
                    parameter.Size = parameter.Value.ToString().Length;
                }

                cmd.Parameters.Add(parameter);
            }
        }

        private static bool ParamIsAnsiString(IDacParameter dacParam)
        {
            const StringComparison c = StringComparison.OrdinalIgnoreCase;

            return dacParam.Name.StartsWith("@mem", c) 
                || dacParam.Name.StartsWith("@inc", c) 
                || dacParam.Name.StartsWith("@tableName", c)
                || dacParam.Name.StartsWith("@entityName", c)
                || dacParam.Name.StartsWith("@dbName", c);
        }

	    public static void ExecuteNonQuery(this IDbConnection connection, string sql, IDbTransaction transaction = null, params IDacParameter[] parameters)
        {
            using(var cmd = connection.CreateCommand(sql, transaction, parameters))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public static void ExecuteNonQuery(this IDbConnection connection, string[] sqls, IDbTransaction transaction = null, params IDacParameter[] parameters)
        {
            using (var cmd = connection.CreateCommand(string.Empty, transaction, parameters))
            {
                foreach (var sqlStatement in sqls.Where(statement => !string.IsNullOrWhiteSpace(statement))) 
                {
                    cmd.CommandText = sqlStatement.Trim();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static T ExecuteScalarResult<T>(this IDbConnection connection, string sql, IDbTransaction transaction = null, params IDacParameter[] parameters)
        {
            using (var cmd = connection.CreateCommand(sql, transaction, parameters))
            {
                return cmd.GetScalarResult<T>();
            }
        }

        public static T GetScalarResult<T>(this IDbCommand cmd)
        {
            var value = cmd.ExecuteScalar();

            if (value == null || value == DBNull.Value)
                return default(T);

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}