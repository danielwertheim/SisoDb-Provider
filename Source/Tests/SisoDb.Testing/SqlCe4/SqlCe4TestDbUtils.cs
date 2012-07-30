using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.EnsureThat;
using SisoDb.NCore;

namespace SisoDb.Testing.SqlCe4
{
    public class SqlCe4TestDbUtils : SqlServerTestDbUtils
    {
        public SqlCe4TestDbUtils(IAdoDriver driver, IConnectionString connectionString) 
            : base(driver, ConvertToConnectionString(connectionString))
        {
        }

        private static string ConvertToConnectionString(IConnectionString connectionString)
        {
            var cnStringBuilder = new SqlCeConnectionStringBuilder(connectionString.PlainString);
            cnStringBuilder.Enlist = false;

            return connectionString.ReplacePlain(cnStringBuilder.ConnectionString).PlainString;
        }

        public override bool TableExists(string name)
        {
            var sql = "select 1 from information_schema.tables where table_name = '{0}';".Inject(name);

            return ExecuteNullableScalar<int>(sql).HasValue;
        }

        public override bool TablesExists(string[] names)
        {
            foreach (var name in names)
            {
                var exists = TableExists(name);
                if (!exists)
                    return false;
            }

            return true;
        }

        public override bool TypeExists(string name)
        {
            var sql = "select 1 from INFORMATION_SCHEMA.PROVIDER_TYPES where TYPE_NAME = '{0}';".Inject(name);

            return ExecuteNullableScalar<int>(sql).HasValue;
        }

        public override IList<DbColumn> GetColumns(string tableName, params string[] namesToSkip)
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

        public override void CreateProcedure(string spSql)
        {
            throw new NotSupportedException("SqlCe4 does not support procedures.");
        }

        public override void DropProcedure(string spName)
        {
            throw new NotSupportedException("SqlCe4 does not support procedures.");
        }
    }
}