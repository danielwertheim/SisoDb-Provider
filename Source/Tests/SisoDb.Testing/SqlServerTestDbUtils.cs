using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.EnsureThat;
using SisoDb.NCore;
using SisoDb.NCore.Expressions;
using SisoDb.PineCone.Structures.Schemas;

namespace SisoDb.Testing
{
    public abstract class SqlServerTestDbUtils : ITestDbUtils
    {
        protected readonly IAdoDriver Driver;
        protected readonly string ConnectionString;

        protected SqlServerTestDbUtils(IAdoDriver driver, string connectionString)
        {
            Driver = driver;
            ConnectionString = connectionString;
        }

        public virtual bool TableExists(string name)
        {
            var sql = "select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME = '{0}';".Inject(name);

            return ExecuteNullableScalar<int>(CommandType.Text, sql).HasValue;
        }

        public virtual bool TablesExists(string[] names)
        {
            foreach (var name in names)
            {
                var exists = TableExists(name);
                if (!exists)
                    return false;
            }

            return true;
        }

        public virtual bool TypeExists(string name)
        {
            var sql = "select 1 from sys.table_types where name = '{0}';".Inject(name);

            return ExecuteNullableScalar<int>(CommandType.Text, sql).HasValue;
        }

        public virtual IList<DbColumn> GetColumns(string tableName, params string[] namesToSkip)
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
                new DacParameter(DbSchemas.Parameters.TableNameParamPrefix, tableName));

            return dbColumns;
        }

        public virtual void CreateProcedure(string spSql)
        {
            ExecuteSql(CommandType.Text, spSql);
        }

        public virtual void DropProcedure(string spName)
        {
            ExecuteSql(CommandType.Text, "if(select OBJECT_ID('{0}', 'P')) is not null begin drop procedure {0}; end".Inject(spName));
        }

        public virtual T ExecuteScalar<T>(CommandType commandType, string sql)
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

        public virtual T? ExecuteNullableScalar<T>(CommandType commandType, string sql) where T : struct
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

        public virtual int RowCount(string tableName, string where = null)
        {
            where = where ?? "1 = 1";
            var sql = "select count(*) from [{0}] where {1};".Inject(tableName, where);

            return ExecuteScalar<int>(CommandType.Text, sql);
        }

        public virtual void DeleteQueryIndexesFor(IStructureSchema structureSchema, IEnumerable<Guid> structureIds)
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

                    foreach (var tableName in indexesTableNames.All)
                    {
                        cmd.CommandText = sqlFormat.Inject(tableName);
                        cmd.ExecuteNonQuery();
                    }
                    cn.Close();
                }
            }
        }

        public virtual bool AnyIndexesTableHasMember<T>(IStructureSchema structureSchema, ValueType id, Expression<Func<T, object>> member) where T : class
        {
            var memberPath = GetMemberPath(member);
            var indexesTableNames = structureSchema.GetIndexesTableNames();
            foreach (var indexesTableName in indexesTableNames.All)
                if (RowCount(indexesTableName, "[{0}] = '{1}'".Inject(IndexStorageSchema.Fields.MemberPath.Name, memberPath)) > 0)
                    return true;

            return false;
        }

        public virtual bool UniquesTableHasMember<T>(IStructureSchema structureSchema, ValueType id, Expression<Func<T, object>> member) where T : class
        {
            var memberPath = GetMemberPath(member);
            return RowCount(structureSchema.GetUniquesTableName(), "[{0}] = '{1}'".Inject(UniqueStorageSchema.Fields.UqMemberPath.Name, memberPath)) > 0;
        }

        public virtual bool IdentityRowExistsForSchema(IStructureSchema structureSchema)
        {
            var sql = "select 1 from SisoDbIdentities where EntityName = '{0}';".Inject(structureSchema.Name);

            return ExecuteNullableScalar<int>(CommandType.Text, sql).HasValue;
        }

        protected virtual string GetMemberPath<T>(Expression<Func<T, object>> e)
        {
            return e.GetRightMostMember().ToPath();
        }

        protected virtual void ExecuteSingleResultSequentialReader(string sql, Action<IDataRecord> recordCallback, params IDacParameter[] parameters)
        {
            using (var cn = CreateConnection())
            {
                cn.Open();

                using (var cmd = Driver.CreateCommand(cn, sql, null, parameters))
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

        protected virtual void ExecuteSql(CommandType commandType, string sql)
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

        protected virtual IDbConnection CreateConnection()
        {
            return Driver.CreateConnection(ConnectionString);
        }
    }
}