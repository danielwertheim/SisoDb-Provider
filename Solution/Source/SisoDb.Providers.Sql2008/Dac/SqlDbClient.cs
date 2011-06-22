using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using Microsoft.SqlServer.Server;
using SisoDb.Commands;
using SisoDb.Core;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.Resources;
using SisoDb.Structures;

namespace SisoDb.Sql2008.Dac
{
    /// <summary>
    /// Performs the ADO.Net communication for the Sql-provider for a
    /// specific database.
    /// </summary>
    public class SqlDbClient : IDisposable
    {
        private SqlConnection _connection;
        private SqlTransaction _transaction;
        private TransactionScope _ts;

        public string DbName
        {
            get { return _connection.Database; }
        }

        public StorageProviders ProviderType { get; private set; }

        public IConnectionString ConnectionString { get; private set; }

        public IDbDataTypeTranslator DbDataTypeTranslator { get; private set; }

        public ISqlStatements SqlStatements { get; private set; }

        public SqlDbClient(SqlConnectionInfo connectionInfo, bool transactional)
        {
            connectionInfo.AssertNotNull("connectionInfo");

            ProviderType = connectionInfo.ProviderType;
            ConnectionString = connectionInfo.ConnectionString;

            SqlStatements = Sql2008Statements.Instance;
            DbDataTypeTranslator = new SqlDbDataTypeTranslator();

            _connection = new SqlConnection(ConnectionString.PlainString);
            _connection.Open();

            if (Transaction.Current == null)
                _transaction = transactional ? _connection.BeginTransaction() : null;
            else
                _ts = new TransactionScope(TransactionScopeOption.Suppress);
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
                _transaction = null;
            }

            if(_ts != null)
            {
                _ts.Dispose();
                _ts = null;
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
            if(_ts != null)
            {
                _ts.Complete();
                _ts.Dispose();
                _ts = new TransactionScope(TransactionScopeOption.Suppress);
                return;
            }

            if (Transaction.Current != null)
                return;

            if (_transaction == null)
                throw new NotSupportedException(ExceptionMessages.SqlDbClient_Flus_NonTransactional);

            _transaction.Commit();
            _transaction.Dispose();
            _transaction = _connection.BeginTransaction();
        }

        public void RebuildIndexes(string structureTableName, string indexesTableName, string uniquesTableName)
        {
            structureTableName.AssertNotNullOrWhiteSpace("structureTableName");
            indexesTableName.AssertNotNullOrWhiteSpace("indexesTableName");
            uniquesTableName.AssertNotNullOrWhiteSpace("uniquesTableName");

            var sql = SqlStatements.GetSql("RebuildIndexes").Inject(
                structureTableName, indexesTableName, uniquesTableName);

            using (var cmd = CreateCommand(CommandType.Text, sql))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public IDbCommand CreateCommand(CommandType commandType, string sql, params IDacParameter[] parameters)
        {
            return _connection.CreateCommand(_transaction, commandType, sql, parameters);
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

            var sql = SqlStatements.GetSql("DeleteById").Inject(
                indexesTableName, uniquesTableName, structureTableName);

            using (var cmd = CreateCommand(CommandType.Text, sql, new DacParameter("id", sisoId)))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteByIds(IEnumerable<ValueType> ids, IdTypes idType, string structureTableName, string indexesTableName, string uniquesTableName)
        {
            structureTableName.AssertNotNullOrWhiteSpace("structureTableName");
            indexesTableName.AssertNotNullOrWhiteSpace("indexesTableName");
            uniquesTableName.AssertNotNullOrWhiteSpace("uniquesTableName");

            var sql = SqlStatements.GetSql("DeleteByIds").Inject(
                indexesTableName, uniquesTableName, structureTableName);

            using (var cmd = CreateCommand(CommandType.Text, sql))
            {
                cmd.Parameters.Add(idType == IdTypes.Identity
                                   ? CreateIdentityIdsTableParam(ids)
                                   : CreateGuidIdsTableParam(ids));

                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteByQuery(ISqlCommandInfo cmdInfo, Type idType, string structureTableName, string indexesTableName, string uniquesTableName)
        {
            structureTableName.AssertNotNullOrWhiteSpace("structureTableName");
            indexesTableName.AssertNotNullOrWhiteSpace("indexesTableName");
            uniquesTableName.AssertNotNullOrWhiteSpace("uniquesTableName");

            var sqlDataType = DbDataTypeTranslator.ToDbType(idType);
            var sql = SqlStatements.GetSql("DeleteByQuery").Inject(
                indexesTableName, uniquesTableName, structureTableName, cmdInfo.Sql, sqlDataType);

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

            var sql = SqlStatements.GetSql("DeleteWhereIdIsBetween").Inject(
                indexesTableName, uniquesTableName, structureTableName);

            using (var cmd = CreateCommand(CommandType.Text, sql, new DacParameter("idFrom", sisoIdFrom), new DacParameter("idTo", sisoIdTo)))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public bool TableExists(string name)
        {
            name.AssertNotNullOrWhiteSpace("name");

            var sql = SqlStatements.GetSql("TableExists");
            var value = ExecuteScalar<string>(CommandType.Text, sql, new DacParameter("tableName", name));

            return !string.IsNullOrWhiteSpace(value);
        }

        public IList<DbColumn> GetColumns(string tableName, params string[] namesToSkip)
        {
            tableName.AssertNotNullOrWhiteSpace("tableName");

            var tmpNamesToSkip = new HashSet<string>(namesToSkip);
            var dbColumns = new List<DbColumn>();

            var sql = SqlStatements.GetSql("GetColumns");

            SingleResultSequentialReader(CommandType.Text, sql,
                dr =>
                {
                    var name = dr.GetString(0);
                    if (!tmpNamesToSkip.Contains(name))
                        dbColumns.Add(new DbColumn(name, dr.GetString(1)));
                },
                new DacParameter("tableName", tableName));

            return dbColumns;
        }

        public int RowCount(string structureTableName)
        {
            structureTableName.AssertNotNullOrWhiteSpace("structureTableName");

            var sql = SqlStatements.GetSql("RowCount").Inject(structureTableName);

            return ExecuteScalar<int>(CommandType.Text, sql);
        }

        public int RowCountByQuery(string indexesTableName, ISqlCommandInfo cmdInfo)
        {
            indexesTableName.AssertNotNullOrWhiteSpace("indexesTableName");

            var sql = SqlStatements.GetSql("RowCountByQuery").Inject(indexesTableName, cmdInfo.Sql);

            return ExecuteScalar<int>(CommandType.Text, sql, cmdInfo.Parameters.ToArray());
        }

        public int CheckOutAndGetNextIdentity(string entityHash, int numOfIds)
        {
            entityHash.AssertNotNullOrWhiteSpace("entityHash");

            var sql = SqlStatements.GetSql("Sys_Identities_CheckOutAndGetNextIdentity");

            return ExecuteScalar<int>(CommandType.Text, sql,
                                                new DacParameter("entityHash", entityHash),
                                                new DacParameter("numOfIds", numOfIds));
        }

        public string GetJsonById(ValueType sisoId, string structureTableName)
        {
            structureTableName.AssertNotNullOrWhiteSpace("structureTableName");

            var sql = SqlStatements.GetSql("GetById").Inject(structureTableName);

            return ExecuteScalar<string>(CommandType.Text, sql, new DacParameter("id", sisoId));
        }

        public IEnumerable<string> GetJsonByIds(IEnumerable<ValueType> ids, IdTypes idType, string structureTableName)
        {
            structureTableName.AssertNotNullOrWhiteSpace("structureTableName");

            var sql = SqlStatements.GetSql("GetByIds").Inject(structureTableName);

            using (var cmd = CreateCommand(CommandType.Text, sql))
            {
                cmd.Parameters.Add(idType == IdTypes.Identity
                                   ? CreateIdentityIdsTableParam(ids)
                                   : CreateGuidIdsTableParam(ids));

                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        yield return reader.GetString(0);
                    }
                    reader.Close();
                }
            }
        }

        public IEnumerable<string> GetJsonWhereIdIsBetween(ValueType sisoIdFrom, ValueType sisoIdTo, string structureTableName)
        {
            structureTableName.AssertNotNullOrWhiteSpace("structureTableName");

            var sql = SqlStatements.GetSql("GetJsonWhereIdIsBetween").Inject(structureTableName);

            using (var cmd = CreateCommand(CommandType.Text, sql, new DacParameter("idFrom", sisoIdFrom), new DacParameter("idTo", sisoIdTo)))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess))
                {
                    while (reader.Read())
                    {
                        yield return reader.GetString(0);
                    }
                    reader.Close();
                }
            }
        }

        private static SqlParameter CreateIdentityIdsTableParam(IEnumerable<ValueType> ids)
        {
            return new SqlParameter("@ids", SqlDbType.Structured)
                           {
                               Value = ids.Select(id => CreateIdentityIdRecord((int)id)),
                               TypeName = "dbo.SisoIdentityIds"
                           };
        }

        private static SqlDataRecord CreateIdentityIdRecord(int id)
        {
            var record = new SqlDataRecord(new SqlMetaData("Id", SqlDbType.Int));

            record.SetInt32(0, id);

            return record;
        }

        private static SqlParameter CreateGuidIdsTableParam(IEnumerable<ValueType> ids)
        {
            return new SqlParameter("@ids", SqlDbType.Structured)
            {
                Value = ids.Select(id => CreateGuidIdRecord((Guid)id)),
                TypeName = "dbo.SisoGuidIds"
            };
        }

        private static SqlDataRecord CreateGuidIdRecord(Guid id)
        {
            var record = new SqlDataRecord(new SqlMetaData("Id", SqlDbType.UniqueIdentifier));

            record.SetGuid(0, id);

            return record;
        }

        private T ExecuteScalar<T>(CommandType commandType, string sql, params IDacParameter[] parameters)
        {
            using (var cmd = CreateCommand(commandType, sql, parameters))
            {
                return cmd.GetScalarResult<T>();
            }
        }

        public void SingleResultSequentialReader(CommandType commandType, string sql,
            Action<IDataRecord> callback, params IDacParameter[] parameters)
        {
            using (var cmd = CreateCommand(commandType, sql, parameters))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess))
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