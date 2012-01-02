using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq.Expressions;
using EnsureThat;
using NCore;
using NCore.Expressions;
using PineCone.Structures.Schemas;
using SisoDb.Core.Io;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Structures;

namespace SisoDb.Testing.SqlCe4
{
    public class SqlCe4TestDbUtils : ITestDbUtils
    {
        private readonly string _connectionString;
        private readonly DbProviderFactory _factory;

        public SqlCe4TestDbUtils(string connectionString)
        {
            _connectionString = connectionString;
            _factory = DbProviderFactories.GetFactory("System.Data.SqlServerCe.4.0");
        }

        private string GetDbFilePath(string name)
        {
            return Path.Combine(_connectionString.ToLower().Replace("data source=", string.Empty), name + ".sdf");
        }

        public void DropDatabaseIfExists(string name)
        {
            var dbFilePath = GetDbFilePath(name);

            IoHelper.DeleteIfFileExists(dbFilePath);
        }

        public void EnsureDbExists(string name)
        {
            var dbFilePath = GetDbFilePath(name);

            if(IoHelper.FileExists(dbFilePath))
                return;

            using (var engine = new SqlCeEngine(Path.Combine(_connectionString, name + ".sdf")))
            {
                engine.CreateDatabase();
            }
        }

        public bool TableExists(string name)
        {
            var sql = "select 1 from information_schema.tables where table_name = '{0}';".Inject(name);

            return ExecuteNullableScalar<int>(CommandType.Text, sql).HasValue;
        }

        public bool TypeExists(string name)
        {
            var sql = "select 1 from INFORMATION_SCHEMA.PROVIDER_TYPES where TYPE_NAME = '{0}';".Inject(name);

            return ExecuteNullableScalar<int>(CommandType.Text, sql).HasValue;
        }

        public IList<DbColumn> GetColumns(string tableName, params string[] namesToSkip)
        {
            Ensure.That(tableName, "tableName").IsNotNullOrWhiteSpace();

            var tmpNamesToSkip = new HashSet<string>(namesToSkip);
            var dbColumns = new List<DbColumn>();

            const string sql = @"select COLUMN_NAME, DATA_TYPE from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = @tableName;";

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
            throw new NotSupportedException("SqlCe4 does not support procedures.");
        }

        public void DropProcedure(string spName)
        {
            throw new NotSupportedException("SqlCe4 does not support procedures.");
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

        private IDbConnection CreateConnection()
        {
            var cn = _factory.CreateConnection();

            if (cn != null)
                cn.ConnectionString = _connectionString;

            return cn;
        }

        private static string GetMemberPath<T>(Expression<Func<T, object>> e)
        {
            return e.GetRightMostMember().ToPath();
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
    }
}