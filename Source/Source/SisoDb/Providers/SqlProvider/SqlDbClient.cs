using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SisoDb.Providers.Shared.DbSchema;
using SisoDb.Providers.SqlProvider.DbSchema;
using SisoDb.Providers.SqlStrings;
using SisoDb.Querying;
using SisoDb.Resources;

namespace SisoDb.Providers.SqlProvider
{
    /// <summary>
    /// Performs the ADO.Net communication for the Sql-provider.
    /// </summary>
    public class SqlDbClient : ISqlDbClient
    {
        private SqlConnection _connection;
        private SqlTransaction _transaction;

        public IDbDataTypeTranslator DbDataTypeTranslator { get; set; }

        public string DbName 
        {
            get { return _connection.Database; }
        }

        public ISisoConnectionInfo ConnectionInfo { get; private set; }

        public StorageProviders ProviderType 
        {
            get { return ConnectionInfo.ProviderType; }
        }

        public ISqlStringsRepository SqlStringsRepository { get; private set; }

        public SqlDbClient(ISisoConnectionInfo connectionInfo, bool transactional)
        {
            ConnectionInfo = connectionInfo.AssertNotNull("connectionInfo");
            SqlStringsRepository = new SqlStringsRepository(ProviderType);
            DbDataTypeTranslator = new SqlDbDataTypeTranslator();
            _connection = new SqlConnection(ConnectionInfo.ConnectionString.PlainString);
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

        public void Flush()
        {
            if(_transaction == null)
                throw new NotSupportedException(ExceptionMessages.SqlDbClient_Flus_NonTransactional);

            _transaction.Commit();
            _transaction.Dispose();
            _transaction = _connection.BeginTransaction();
        }

        public SqlBulkCopy GetBulkCopy(bool keepIdentities)
        {
            SqlBulkCopyOptions options = keepIdentities ? SqlBulkCopyOptions.KeepIdentity : SqlBulkCopyOptions.Default;

            return new SqlBulkCopy(_connection, options, _transaction);
        }

        public void CreateDatabase(string name)
        {
            var sql = SqlStringsRepository.GetSql("CreateDatabase").Inject(name);

            using (var cmd = CreateCommand(CommandType.Text, sql))
            {
                cmd.ExecuteNonQuery();

                ExecuteCreateSysTables(name, cmd);
            }
        }

        public void CreateSysTables(string name)
        {
            using (var cmd = CreateCommand(CommandType.Text, string.Empty))
            {
                ExecuteCreateSysTables(name, cmd);
            }
        }

        private void ExecuteCreateSysTables(string name, IDbCommand cmd)
        {
            cmd.CommandText = SqlStringsRepository.GetSql("Sys_Identities_CreateIfNotExists").Inject(name);
            cmd.ExecuteNonQuery();
        }

        public void DropDatabase(string name)
        {
            var sql = SqlStringsRepository.GetSql("DropDatabase").Inject(name);

            ExecuteNonQuery(CommandType.Text, sql);
        }

        public bool DatabaseExists(string name)
        {
            var sql = SqlStringsRepository.GetSql("DatabaseExists");

            var value = ExecuteScalar<int>(CommandType.Text, sql, new QueryParameter("dbName", name));

            return value > 0;
        }

        public bool TableExists(string name)
        {
            var sql = SqlStringsRepository.GetSql("TableExists");
            var value = ExecuteScalar<string>(CommandType.Text, sql, new QueryParameter("tableName", name));

            return !string.IsNullOrWhiteSpace(value);
        }

        public IList<SqlDbColumn> GetColumns(string tableName, params string[] namesToSkip)
        {
            var tmpNamesToSkip = new HashSet<string>(namesToSkip);
            var dbColumns = new List<SqlDbColumn>();

            var sql = SqlStringsRepository.GetSql("GetColumns");

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

        public int RowCount(string tableName)
        {
            var sql = SqlStringsRepository.GetSql("RowCount").Inject(tableName);

            return ExecuteScalar<int>(CommandType.Text, sql);
        }

        public int GetIdentity(string entityHash, int numOfIds)
        {
            var sql = SqlStringsRepository.GetSql("Sys_Identities_Get");

            return ExecuteScalar<int>(CommandType.Text, sql,
                                                new QueryParameter("entityHash", entityHash),
                                                new QueryParameter("numOfIds", numOfIds));
        }

        public void DeleteById(ValueType structureId, string structureTableName, string indexesTableName, string uniquesTableName)
        {
            structureTableName.AssertNotNullOrWhiteSpace("structureTableName");
            indexesTableName.AssertNotNullOrWhiteSpace("indexesTableName");
            uniquesTableName.AssertNotNullOrWhiteSpace("uniquesTableName");

            var sql = SqlStringsRepository.GetSql("DeleteById").Inject(
                structureTableName, indexesTableName, uniquesTableName);

            ExecuteNonQuery(CommandType.Text, sql, new QueryParameter("id", structureId));
        }

        public void DeleteByQuery(ISqlCommandInfo cmdInfo, Type idType, string structureTableName, string indexesTableName, string uniquesTableName)
        {
            structureTableName.AssertNotNullOrWhiteSpace("structureTableName");
            indexesTableName.AssertNotNullOrWhiteSpace("indexesTableName");
            uniquesTableName.AssertNotNullOrWhiteSpace("uniquesTableName");
            var sqlDataType = DbDataTypeTranslator.ToDbType(idType);
            var sql = SqlStringsRepository.GetSql("DeleteByQuery").Inject(indexesTableName, uniquesTableName, structureTableName, cmdInfo.Value, sqlDataType);

            ExecuteNonQuery(CommandType.Text, sql, cmdInfo.Parameters.ToArray());
        }

        public void DeleteWhereIdIsBetween(ValueType structureIdFrom, ValueType structureIdTo, string structureTableName, string indexesTableName, string uniquesTableName)
        {
            structureTableName.AssertNotNullOrWhiteSpace("structureTableName");
            indexesTableName.AssertNotNullOrWhiteSpace("indexesTableName");
            uniquesTableName.AssertNotNullOrWhiteSpace("uniquesTableName");

            var sql = SqlStringsRepository.GetSql("DeleteWhereIdIsBetween").Inject(
                structureTableName, indexesTableName, uniquesTableName);

            ExecuteNonQuery(CommandType.Text, sql, new QueryParameter("idFrom", structureIdFrom), new QueryParameter("idTo", structureIdTo));
        }

        public string GetJsonById(ValueType structureId, string structureTableName)
        {
            var sql = SqlStringsRepository.GetSql("GetById").Inject(structureTableName);

            return ExecuteScalar<string>(CommandType.Text, sql, new QueryParameter("id", structureId));
        }

        public T ExecuteScalar<T>(CommandType commandType, string sql, params IQueryParameter[] parameters)
        {
            using (var cmd = CreateCommand(commandType, sql, parameters))
            {
                var value = cmd.ExecuteScalar();

                if (value == null || value == DBNull.Value)
                    return default(T);

                return (T)Convert.ChangeType(value, typeof(T));
            }
        }

        public void ExecuteSingleResultReader(CommandType commandType, string sql,
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

        public int ExecuteNonQuery(CommandType commandType, string sql, params IQueryParameter[] parameters)
        {
            int affectedRowsCount;

            using (var cmd = CreateCommand(commandType, sql, parameters))
            {
                affectedRowsCount = cmd.ExecuteNonQuery();
            }

            return affectedRowsCount;
        }

        public IDbCommand CreateCommand(CommandType commandType, string sql, params IQueryParameter[] parameters)
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