using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using SisoDb.Core;

namespace SisoDb.Tests.IntegrationTests.SqlCe4
{
    public class SqlCe4DbUtils
    {
        private readonly string _connectionString;
        private readonly DbProviderFactory _factory;

        internal SqlCe4DbUtils(string connectionString)
        {
            _connectionString = connectionString;
            _factory = DbProviderFactories.GetFactory("System.Data.SqlServerCe.4.0");
        }

        internal void CreateEmptyDb()
        {
            using (var engine = new SqlCeEngine(_connectionString))
            {
                engine.CreateDatabase();
            }
        }

        internal bool TableExists(string name)
        {
            var sql = "select 1 from information_schema.tables where table_name = '{0}';".Inject(name);

            return ExecuteNullableScalar<int>(CommandType.Text, sql).HasValue;
        }

        internal T? ExecuteNullableScalar<T>(CommandType commandType, string sql) where T : struct
        {
            T? retVal;

            using (var cn = CreateConnection())
            {
                cn.Open();
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandType = commandType;
                    cmd.CommandText = sql;

                    var value = cmd.ExecuteScalar();

                    if (value == null || value == DBNull.Value)
                        retVal = default(T?);
                    else
                        retVal = (T)Convert.ChangeType(value, typeof(T));
                }
                cn.Close();
            }

            return retVal;
        }

        private IDbConnection CreateConnection()
        {
            var cn = _factory.CreateConnection();
            cn.ConnectionString = _connectionString;

            return cn;
        }
    }
}