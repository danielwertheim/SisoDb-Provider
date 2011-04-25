using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SisoDb.Core;
using SisoDb.Providers.DbSchema;
using SisoDb.Providers.SqlProvider.DbSchema;
using SisoDb.Providers.SqlStrings;
using SisoDb.Querying;
using SisoDb.Resources;

namespace SisoDb.Providers.SqlProvider
{
    /// <summary>
    /// Performs the ADO.Net communication for the Sql-provider for a
    /// specific database.
    /// </summary>
    public class SqlDbClient : ISqlDbClient
    {
        private SqlConnection _connection;
        private SqlTransaction _transaction;

        public string DbName
        {
            get { return _connection.Database; }
        }

        public StorageProviders ProviderType
        {
            get { return ConnectionInfo.ProviderType; }
        }

        public ISisoConnectionInfo ConnectionInfo { get; private set; }

        public IDbDataTypeTranslator DbDataTypeTranslator { get; private set; }

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
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
                _transaction = null;
            }

            if (_connection == null)
                return;

            if (_connection.State != ConnectionState.Closed)
                _connection.Close();

            _connection.Dispose();
            _connection = null;
        }

        public void Flush()
        {
            if (_transaction == null)
                throw new NotSupportedException(ExceptionMessages.SqlDbClient_Flus_NonTransactional);

            _transaction.Commit();
            _transaction.Dispose();
            _transaction = _connection.BeginTransaction();
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

        public SqlBulkCopy GetBulkCopy(bool keepIdentities)
        {
            var options = keepIdentities ? SqlBulkCopyOptions.KeepIdentity : SqlBulkCopyOptions.Default;

            return new SqlBulkCopy(_connection, options, _transaction);
        }

        public void DeleteById(ValueType sisoId, string structureTableName, string indexesTableName, string uniquesTableName)
        {
            structureTableName.AssertNotNullOrWhiteSpace("structureTableName");
            indexesTableName.AssertNotNullOrWhiteSpace("indexesTableName");
            uniquesTableName.AssertNotNullOrWhiteSpace("uniquesTableName");

            var sql = SqlStringsRepository.GetSql("DeleteById").Inject(
                structureTableName, indexesTableName, uniquesTableName);

            using (var cmd = CreateCommand(CommandType.Text, sql, new QueryParameter("id", sisoId)))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteByQuery(ISqlCommandInfo cmdInfo, Type idType, string structureTableName, string indexesTableName, string uniquesTableName)
        {
            structureTableName.AssertNotNullOrWhiteSpace("structureTableName");
            indexesTableName.AssertNotNullOrWhiteSpace("indexesTableName");
            uniquesTableName.AssertNotNullOrWhiteSpace("uniquesTableName");
            var sqlDataType = DbDataTypeTranslator.ToDbType(idType);
            var sql = SqlStringsRepository.GetSql("DeleteByQuery").Inject(indexesTableName, uniquesTableName, structureTableName, cmdInfo.Sql, sqlDataType);

            using (var cmd = CreateCommand(CommandType.Text, sql, cmdInfo.Parameters.ToArray()))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteWhereIdIsBetween(ValueType sisoIdFrom, ValueType sisoIdTo, string structureTableName, string indexesTableName, string uniquesTableName)
        {
            structureTableName.AssertNotNullOrWhiteSpace("structureTableName");
            indexesTableName.AssertNotNullOrWhiteSpace("indexesTableName");
            uniquesTableName.AssertNotNullOrWhiteSpace("uniquesTableName");

            var sql = SqlStringsRepository.GetSql("DeleteWhereIdIsBetween").Inject(
                structureTableName, indexesTableName, uniquesTableName);

            using (var cmd = CreateCommand(CommandType.Text, sql, new QueryParameter("idFrom", sisoIdFrom), new QueryParameter("idTo", sisoIdTo)))
            {
                cmd.ExecuteNonQuery();
            }
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
                    if (!tmpNamesToSkip.Contains(name))
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

        public int CheckOutAndGetNextIdentity(string entityHash, int numOfIds)
        {
            var sql = SqlStringsRepository.GetSql("Sys_Identities_CheckOutAndGetNextIdentity");

            return ExecuteScalar<int>(CommandType.Text, sql,
                                                new QueryParameter("entityHash", entityHash),
                                                new QueryParameter("numOfIds", numOfIds));
        }

        public string GetJsonById(ValueType sisoId, string structureTableName)
        {
            var sql = SqlStringsRepository.GetSql("GetById").Inject(structureTableName);

            return ExecuteScalar<string>(CommandType.Text, sql, new QueryParameter("id", sisoId));
        }

        private T ExecuteScalar<T>(CommandType commandType, string sql, params IQueryParameter[] parameters)
        {
            using (var cmd = CreateCommand(commandType, sql, parameters))
            {
                return cmd.GetScalarResult<T>();
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
    }
}