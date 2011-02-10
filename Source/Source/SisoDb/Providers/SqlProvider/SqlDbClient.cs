using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SisoDb.Providers.Sql;
using SisoDb.Providers.SqlProvider.DbSchema;
using SisoDb.Querying;

namespace SisoDb.Providers.SqlProvider
{
    /// <summary>
    /// Performs the ADO.Net communication for the Sql-provider.
    /// </summary>
    internal class SqlDbClient : IDisposable
    {
        private SqlConnection _connection;
        private SqlTransaction _transaction;
        private SqlDbDataTypeTranslator _dataTypeTranslator;

        internal string DbName 
        {
            get { return _connection.Database; }
        }

        internal StorageProviders ProviderType { get; private set; }

        internal ISqlStrings SqlStrings { get; private set; }

        internal SqlDbClient(ISisoConnectionInfo connectionInfo, bool transactional)
        {
            ProviderType = connectionInfo.ProviderType;
            SqlStrings = new SqlStrings(ProviderType);
            _dataTypeTranslator = new SqlDbDataTypeTranslator();
            _connection = new SqlConnection(connectionInfo.ConnectionString.PlainString);
            _connection.Open();

            _transaction = transactional ?
                _connection.BeginTransaction() : null;
        }

        public void Dispose()
        {
            if(_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
                _transaction = null;
            }

            if(_connection != null)
            {
                if (_connection.State != ConnectionState.Closed)
                    _connection.Close();

                _connection = null;
            }
        }

        internal void Flush()
        {
            _transaction.Commit();
            _transaction.Dispose();
            _transaction = _connection.BeginTransaction();
        }

        internal SqlBulkCopy GetBulkCopy()
        {
            return new SqlBulkCopy(_connection, SqlBulkCopyOptions.Default, _transaction);
        }

        internal void CreateDatabase(string name)
        {
            var sql = SqlStrings.GetSql("CreateDatabase").Inject(name);

            using (var cmd = CreateCommand(CommandType.Text, sql))
            {
                cmd.ExecuteNonQuery();

                ExecuteCreateSysTables(name, cmd);
            }
        }

        internal void CreateSysTables(string name)
        {
            using (var cmd = CreateCommand(CommandType.Text, string.Empty))
            {
                ExecuteCreateSysTables(name, cmd);
            }
        }

        private void ExecuteCreateSysTables(string name, IDbCommand cmd)
        {
            cmd.CommandText = SqlStrings.GetSql("Sys_Identities_CreateIfNotExists").Inject(name);
            cmd.ExecuteNonQuery();
        }

        internal void DropDatabase(string name)
        {
            var sql = SqlStrings.GetSql("DropDatabase").Inject(name);

            ExecuteNonQuery(CommandType.Text, sql);
        }

        internal bool DatabaseExists(string name)
        {
            var sql = SqlStrings.GetSql("DatabaseExists");

            var value = ExecuteScalar<int>(CommandType.Text, sql, new QueryParameter("dbName", name));

            return value > 0;
        }

        internal bool TableExists(string name)
        {
            var sql = SqlStrings.GetSql("TableExists");
            var value = ExecuteScalar<string>(CommandType.Text, sql, new QueryParameter("tableName", name));

            return !string.IsNullOrWhiteSpace(value);
        }

        internal IList<SqlDbColumn> GetColumns(string tableName, params string[] namesToSkip)
        {
            var tmpNamesToSkip = new HashSet<string>(namesToSkip);
            var dbColumns = new List<SqlDbColumn>();

            var sql = SqlStrings.GetSql("GetColumns");

            ExecuteSingleResultReader(CommandType.Text, sql,
                dr =>
                    {
                        var name = dr.GetString(0);
                        if(!tmpNamesToSkip.Contains(name))
                            dbColumns.Add(new SqlDbColumn(name, dr.GetString(1)));
                    },
                new QueryParameter("tableName", tableName));

            return dbColumns;
        }

        internal int RowCount(string tableName)
        {
            var sql = SqlStrings.GetSql("RowCount").Inject(tableName);

            return ExecuteScalar<int>(CommandType.Text, sql);
        }

        internal int GetIdentity(string entityHash, int numOfIds)
        {
            var sql = SqlStrings.GetSql("Sys_Identities_Get");

            return ExecuteScalar<int>(CommandType.Text, sql,
                                                new QueryParameter("entityHash", entityHash),
                                                new QueryParameter("numOfIds", numOfIds));
        }

        internal void DeleteById(ValueType structureId, string structureTableName, string indexesTableName, string uniquesTableName)
        {
            structureTableName.AssertNotNullOrWhiteSpace("structureTableName");
            indexesTableName.AssertNotNullOrWhiteSpace("indexesTableName");
            uniquesTableName.AssertNotNullOrWhiteSpace("uniquesTableName");

            var sql = SqlStrings.GetSql("DeleteById").Inject(
                structureTableName, indexesTableName, uniquesTableName);

            ExecuteNonQuery(CommandType.Text, sql, new QueryParameter("id", structureId));
        }

        internal void DeleteByQuery(ISqlCommandInfo cmdInfo, Type idType, string structureTableName, string indexesTableName, string uniquesTableName)
        {
            structureTableName.AssertNotNullOrWhiteSpace("structureTableName");
            indexesTableName.AssertNotNullOrWhiteSpace("indexesTableName");
            uniquesTableName.AssertNotNullOrWhiteSpace("uniquesTableName");
            var sqlDataType = _dataTypeTranslator.ToDbType(idType);
            var sql = SqlStrings.GetSql("DeleteByQuery").Inject(indexesTableName, uniquesTableName, structureTableName, cmdInfo.Value, sqlDataType);

            ExecuteNonQuery(CommandType.Text, sql, cmdInfo.Parameters.ToArray());
        }

        internal string GetJsonById(ValueType structureId, string structureTableName)
        {
            var sql = SqlStrings.GetSql("GetById").Inject(structureTableName);

            return ExecuteScalar<string>(CommandType.Text, sql, new QueryParameter("id", structureId));
        }

        internal T ExecuteScalar<T>(CommandType commandType, string sql, params IQueryParameter[] parameters)
        {
            using (var cmd = CreateCommand(commandType, sql, parameters))
            {
                var value = cmd.ExecuteScalar();

                if (value == null || value == DBNull.Value)
                    return default(T);

                return (T)Convert.ChangeType(value, typeof(T));
            }
        }

        internal void ExecuteSingleResultReader(CommandType commandType, string sql,
            Action<IDataRecord> callback, params IQueryParameter[] parameters)
        {
            using (var cmd = CreateCommand(commandType, sql, parameters))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        callback(reader);
                    }
                    reader.Close();
                }
            }
        }

        internal int ExecuteNonQuery(CommandType commandType, string sql, params IQueryParameter[] parameters)
        {
            int affectedRowsCount;

            using (var cmd = CreateCommand(commandType, sql, parameters))
            {
                affectedRowsCount = cmd.ExecuteNonQuery();
            }

            return affectedRowsCount;
        }

        internal IDbCommand CreateCommand(CommandType commandType, string sql, params IQueryParameter[] parameters)
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandType = commandType;

            if (!string.IsNullOrWhiteSpace(sql))
                cmd.CommandText = sql;

            if (_transaction != null)
                cmd.Transaction = _transaction;

            foreach (var queryParameter in parameters)
            {
                var parameter = cmd.CreateParameter();
                parameter.ParameterName = queryParameter.Name;
                parameter.Value = queryParameter.Value;

                cmd.Parameters.Add(parameter);
            }

            return cmd;
        }
    }
}