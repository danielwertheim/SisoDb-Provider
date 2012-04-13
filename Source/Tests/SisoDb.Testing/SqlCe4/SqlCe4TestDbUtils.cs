using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
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
        private readonly IAdoDriver _driver;
        private readonly string _connectionString;
        
        public SqlCe4TestDbUtils(IAdoDriver driver, IConnectionString connectionString)
        {
            var cnStringBuilder = new SqlCeConnectionStringBuilder(connectionString.PlainString);
            cnStringBuilder.Enlist = false;

            _driver = driver;
            _connectionString = connectionString.ReplacePlain(cnStringBuilder.ConnectionString).PlainString;
        }

        public bool TableExists(string name)
        {
            var sql = "select 1 from information_schema.tables where table_name = '{0}';".Inject(name);

            return ExecuteNullableScalar<int>(CommandType.Text, sql).HasValue;
        }

        public bool TablesExists(string[] names)
        {
            foreach (var name in names)
            {
                var exists = TableExists(name);
                if (!exists)
                    return false;
            }

            return true;
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
                new DacParameter(DbSchemas.Parameters.TableNameParamPrefix, tableName));

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

        public void DeleteQueryIndexesFor(IStructureSchema structureSchema, IEnumerable<Guid> structureIds)
        {
            var guidIds = string.Join(",", structureIds.Select(id => string.Format("'{0}'", id)));
            var sqlFormat = "delete from [{0}] where StructureId in({1});".Inject("{0}", guidIds);
            var indexesTableNames = structureSchema.GetIndexesTableNames();

            using (var cn = CreateConnection())
            {
                cn.Open();
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;

                    foreach (var tableName in indexesTableNames.AllTableNames)
                    {
                        cmd.CommandText = sqlFormat.Inject(tableName);
                        cmd.ExecuteNonQuery();
                    }
                    cn.Close();
                }
            }
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

        public bool IdentityRowExistsForSchema(IStructureSchema structureSchema)
        {
            var sql = "select 1 from SisoDbIdentities where EntityName = '{0}';".Inject(structureSchema.Name);

            return ExecuteNullableScalar<int>(CommandType.Text, sql).HasValue;
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

                using (var cmd = _driver.CreateCommand(cn, sql, null, parameters))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

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

        private IDbConnection CreateConnection()
        {
            return _driver.CreateConnection(_connectionString);
        }
    }
}