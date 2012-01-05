using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using EnsureThat;
using NCore;
using NCore.Expressions;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;

namespace SisoDb.Testing.Sql2008
{
    public class Sql2008TestDbUtils : ITestDbUtils
    {
        private readonly string _connectionString;
        private readonly DbProviderFactory _factory;

        public Sql2008TestDbUtils(string connectionString)
        {
            _connectionString = connectionString;
            _factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
        }

        public void DropDatabaseIfExists(string name)
        {
            var sql = @"if (select DB_ID('{0}')) is not null begin
                 alter database [{0}] set offline with rollback immediate;
                 alter database [{0}] set online;
                 drop database [{0}]; end".Inject(name);

            ExecuteSql(CommandType.Text, sql);
        }

        public void EnsureDbExists(string name)
        {
            var sql = @"if (select DB_ID('{0}')) is null begin 
                create database [{0}]; 
                alter database [{0}] set recovery simple; end".Inject(name);

            ExecuteSql(CommandType.Text, sql);
        }

        public bool TableExists(string name)
        {
			var sql = "select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = '{0}';".Inject(name);

            return ExecuteNullableScalar<int>(CommandType.Text, sql).HasValue;
        }

        public bool TypeExists(string name)
        {
            var sql = "select 1 from sys.table_types where name = '{0}';".Inject(name);

            return ExecuteNullableScalar<int>(CommandType.Text, sql).HasValue;
        }

        public IList<DbColumn> GetColumns(string tableName, params string[] namesToSkip)
        {
            Ensure.That(tableName, "tableName").IsNotNullOrWhiteSpace();

            var tmpNamesToSkip = new HashSet<string>(namesToSkip);
            var dbColumns = new List<DbColumn>();

            const string sql = @"set nocount on;
                        if (select OBJECT_ID(@tableName, 'U')) is not null
                        begin
                        select
                        COLUMN_NAME,
                        DATA_TYPE
                        from INFORMATION_SCHEMA.COLUMNS
                        where TABLE_NAME = @tableName;
                        end";

            ExecuteSingleResultSequentialReader(sql,
                dr =>
                {
                    var name = dr.GetString(0);
                    if (!tmpNamesToSkip.Contains(name))
                        dbColumns.Add(new DbColumn(name, dr.GetString(1)));
                },
                new DacParameter("tableName", tableName));

            return dbColumns;
        }

        public void CreateProcedure(string spSql)
        {
            ExecuteSql(CommandType.Text, spSql);
        }

        public void DropProcedure(string spName)
        {
            ExecuteSql(CommandType.Text, "if(select OBJECT_ID('{0}', 'P')) is not null begin drop procedure {0}; end".Inject(spName));
        }

        private void ExecuteSingleResultSequentialReader(string sql, Action<IDataRecord> recordCallback, params IDacParameter[] parameters)
        {
            using (var cn = CreateConnection())
            {
                cn.Open();

                using (var cmd = cn.CreateCommand(sql, parameters))
                {
                    using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess))
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

        private void ExecuteSql(CommandType commandType, string sql)
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

        public T ExecuteScalar<T>(CommandType commandType, string sql)
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

        public T? ExecuteNullableScalar<T>(CommandType commandType, string sql) where T : struct
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

            if(cn != null)
                cn.ConnectionString = _connectionString;

            return cn;
        }

//        public bool IndexExists(string tableName, string indexName)
//        {
//            var sql =
//                @"
//declare @tableName varchar(128);
//declare @tableId int;
//
//set @tableName = '{0}';
//set @tableId = OBJECT_ID(@tableName);
//
//declare @count int;
//
//set @count = (select count(*) from sys.indexes where object_id = @tableId and name = '{1}');
//set @count = coalesce(@count, 0);
//select case when @count > 0 then 1 else 0 end;".Inject(tableName, indexName);

//            return ExecuteScalar<bool>(CommandType.Text, sql);
//        }

        public int RowCount(string tableName, string where = null)
        {
            where = where ?? "1 = 1";
            var sql = "select count(*) from [{0}] where {1};".Inject(tableName, where);

            return ExecuteScalar<int>(CommandType.Text, sql);
        }

        public bool IndexesTableHasMember<T>(IStructureSchema structureSchema, ValueType id, Expression<Func<T, object>> member) where T : class
        {
            var memberPath = GetMemberPath(member);
            return RowCount(structureSchema.GetIndexesTableName(), "[{0}] = '{1}'".Inject(IndexStorageSchema.Fields.MemberPath.Name, memberPath)) > 0;
        }

        public bool UniquesTableHasMember<T>(IStructureSchema structureSchema, ValueType id, Expression<Func<T, object>> member) where T : class
        {
            var memberPath = GetMemberPath(member);
            return RowCount(structureSchema.GetUniquesTableName(), "[{0}] = '{1}'".Inject(UniqueStorageSchema.Fields.UqMemberPath.Name, memberPath)) > 0;
        }

        private static string GetMemberPath<T>(Expression<Func<T, object>> e)
        {
            return e.GetRightMostMember().ToPath();
        }
    }
}