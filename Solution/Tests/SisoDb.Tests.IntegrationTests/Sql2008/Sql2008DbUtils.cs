using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using SisoDb.Core;

namespace SisoDb.Tests.IntegrationTests.Sql2008
{
    public class Sql2008DbUtils
    {
        private readonly string _connectionString;
        private readonly DbProviderFactory _factory;

        internal Sql2008DbUtils(string connectionString)
        {
            _connectionString = connectionString;
            _factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
        }

        internal void DropDatabase(string name)
        {
            var sql = @"if (select DB_ID('{0}')) is not null begin
                 alter database [{0}] set offline with rollback immediate;
                 alter database [{0}] set online;
                 drop database [{0}]; end".Inject(name);

            ExecuteSql(CommandType.Text, sql);
        }

        internal void EnsureDbExists(string name)
        {
            var sql = @"if (select DB_ID('{0}')) is null begin 
                create database [{0}]; 
                alter database [{0}] set recovery simple; end".Inject(name);

            ExecuteSql(CommandType.Text, sql);
        }

        internal bool TableExists(string name)
        {
            var sql = "select 1 from information_schema.tables where table_name = '{0}';".Inject(name);

            return ExecuteNullableScalar<int>(CommandType.Text, sql).HasValue;
        }

        internal bool TypeExists(string name)
        {
            var sql = "select 1 from sys.table_types where name = '{0}';".Inject(name);

            return ExecuteNullableScalar<int>(CommandType.Text, sql).HasValue;
        }

        internal void CreateProcedure(string spSql)
        {
            ExecuteSql(CommandType.Text, spSql);
        }

        internal void DropProcedure(string spName)
        {
            ExecuteSql(CommandType.Text, "if(select OBJECT_ID('{0}', 'P')) is not null begin drop procedure {0}; end".Inject(spName));
        }

        internal void AddColumns(string tableName, params string[] columns)
        {
            var columnString = string.Join(",", columns);
            var sql = "alter table [{0}] add {1};".Inject(tableName, columnString);

            ExecuteSql(CommandType.Text, sql);
        }

        internal void DropColumns(string tableName, params string[] columnNames)
        {
            var columnString = string.Join(",", columnNames.Select(c => "[" + c + "]"));
            var sql = "alter table [{0}] drop column {1};".Inject(tableName, columnString);

            ExecuteSql(CommandType.Text, sql);
        }

        internal bool ColumnsExist(string tableName, params string[] columnNames)
        {
            var sql = "select Column_Name from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '{0}';".Inject(tableName);
            var existingColums = new HashSet<string>();

            ExecuteSingleResultReader(CommandType.Text, sql, 
                dr => existingColums.Add(dr.GetString(0)));

            var notExisting = columnNames.Count(c => !existingColums.Contains(c));

            return notExisting < 1;
        }

        internal DataTable GetTableBySql(string sql)
        {
            var table = new DataTable();

            using (var cn = CreateConnection())
            {
                cn.Open();

                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        table.Load(reader);
                        reader.Close();
                    }
                }

                cn.Close();
            }

            return table;
        }

        private void ExecuteSingleResultReader(CommandType commandType, string sql, Action<IDataRecord> recordCallback)
        {
            using (var cn = CreateConnection())
            {
                cn.Open();

                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandType = commandType;
                    cmd.CommandText = sql;

                    using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (reader.Read())
                        {
                            recordCallback(reader);
                        }
                        reader.Close();
                    }
                }

                cn.Close();
            }
        }

        internal void ExecuteSql(CommandType commandType, string sql)
        {
            using (var cn = CreateConnection())
            {
                cn.Open();
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandType = commandType;
                    cmd.CommandText = sql;

                    cmd.ExecuteNonQuery();
                }
                cn.Close();
            }
        }

        internal T ExecuteScalar<T>(CommandType commandType, string sql)
        {
            T retVal;

            using (var cn = CreateConnection())
            {
                cn.Open();
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandType = commandType;
                    cmd.CommandText = sql;

                    var value = cmd.ExecuteScalar();

                    if (value == null || value == DBNull.Value)
                        retVal = default(T);
                    else
                        retVal = (T)Convert.ChangeType(value, typeof(T));
                }
                cn.Close();
            }

            return retVal;
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

        internal bool IndexExists(string tableName, string indexName)
        {
            var sql =
                @"
declare @tableName varchar(128);
declare @tableId int;

set @tableName = '{0}';
set @tableId = OBJECT_ID(@tableName);

declare @count int;

set @count = (select count(*) from sys.indexes where object_id = @tableId and name = '{1}');
set @count = coalesce(@count, 0);
select case when @count > 0 then 1 else 0 end;".Inject(tableName, indexName);

            return ExecuteScalar<bool>(CommandType.Text, sql);
        }

        internal int RowCount(string tableName, string where = null)
        {
            where = where ?? "1 = 1";
            var sql = "select count(*) from dbo.[{0}] where {1};".Inject(tableName, where);

            return ExecuteScalar<int>(CommandType.Text, sql);
        }
    }
}