using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.Linq.Expressions;
using EnsureThat;
using NCore;
using NCore.Expressions;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;

namespace SisoDb.Testing.SqlCe4
{
    public class SqlCe4TestDbUtils : ITestDbUtils
    {
        private readonly IConnectionString _connectionString;
        private readonly DbProviderFactory _factory;

        public SqlCe4TestDbUtils(IConnectionString connectionString)
        {
            var cnStringBuilder = new SqlCeConnectionStringBuilder(connectionString.PlainString);
            cnStringBuilder.Enlist = false;

            _connectionString = connectionString.ReplacePlain(cnStringBuilder.ConnectionString);
            _factory = DbProviderFactories.GetFactory("System.Data.SqlServerCe.4.0");
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

        public bool AnyIndexesTableHasMember<T>(IStructureSchema structureSchema, ValueType id, Expression<Func<T, object>> member) where T : class
        {
			var memberPath = GetMemberPath(member);
			var indexesTableNames = structureSchema.GetIndexesTableNames();
			foreach (var indexesTableName in indexesTableNames.AllTableNames)
				if (RowCount(indexesTableName, "[{0}] = '{1}'".Inject(IndexStorageSchema.Fields.MemberPath.Name, memberPath)) > 0)
					return true;

			return false;
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
                cn.ConnectionString = _connectionString.PlainString;

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

                using (var cmd = cn.CreateCommand(sql, null, parameters))
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