using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using EnsureThat;
using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.Querying.Sql;
using SisoDb.Resources;
using SisoDb.Sql2008.DbSchema;
using SisoDb.Structures;

namespace SisoDb.Sql2008.Dac
{
    /// <summary>
    /// Performs the ADO.Net communication for the Sql2008-provider for a
    /// specific database.
    /// </summary>
    public class Sql2008DbClient : IDbClient
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

        public Sql2008DbClient(ISisoConnectionInfo connectionInfo, bool transactional)
        {
            Ensure.That(() => connectionInfo).IsNotNull();

            ProviderType = connectionInfo.ProviderType;
            ConnectionString = connectionInfo.ConnectionString;

            SqlStatements = Sql2008Statements.Instance;
            DbDataTypeTranslator = new Sql2008DataTypeTranslator();

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

            if (_ts != null)
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
            if (_ts != null)
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
        
        public IDbCommand CreateCommand(CommandType commandType, string sql, params IDacParameter[] parameters)
        {
            return _connection.CreateCommand(_transaction, commandType, sql, parameters);
        }

        public SqlBulkCopy GetBulkCopy(bool keepIdentities)
        {
            var options = keepIdentities ? SqlBulkCopyOptions.KeepIdentity : SqlBulkCopyOptions.Default;

            return new SqlBulkCopy(_connection, options, _transaction);
        }

        public void Drop(IStructureSchema structureSchema)
        {
            var sql = SqlStatements.GetSql("DropStructureTables").Inject(
                structureSchema.GetIndexesTableName(),
                structureSchema.GetUniquesTableName(),
                structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(CommandType.Text, sql, new DacParameter("entityHash", structureSchema.Hash)))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public void RebuildIndexes(IStructureSchema structureSchema)
        {
            Ensure.That(() => structureSchema).IsNotNull();

            var sql = SqlStatements.GetSql("RebuildIndexes").Inject(
                structureSchema.GetStructureTableName(),
                structureSchema.GetIndexesTableName(),
                structureSchema.GetUniquesTableName());

            using (var cmd = CreateCommand(CommandType.Text, sql))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteById(ValueType structureId, IStructureSchema structureSchema)
        {
            Ensure.That(() => structureSchema).IsNotNull();

            var sql = SqlStatements.GetSql("DeleteById").Inject(
                structureSchema.GetIndexesTableName(),
                structureSchema.GetUniquesTableName(),
                structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(CommandType.Text, sql, new DacParameter("id", structureId)))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteByIds(IEnumerable<ValueType> ids, StructureIdTypes idType, IStructureSchema structureSchema)
        {
            Ensure.That(() => structureSchema).IsNotNull();

            var sql = SqlStatements.GetSql("DeleteByIds").Inject(
                structureSchema.GetIndexesTableName(),
                structureSchema.GetUniquesTableName(),
                structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(CommandType.Text, sql))
            {
                cmd.Parameters.Add(Sql2008IdsTableParam.CreateIdsTableParam(idType, ids));
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteByQuery(SqlQuery query, Type idType, IStructureSchema structureSchema)
        {
            Ensure.That(() => structureSchema).IsNotNull();

            var sqlDataType = DbDataTypeTranslator.ToDbType(idType);
            var sql = SqlStatements.GetSql("DeleteByQuery").Inject(
                structureSchema.GetIndexesTableName(),
                structureSchema.GetUniquesTableName(),
                structureSchema.GetStructureTableName(),
                query.Sql,
                sqlDataType);

            using (var cmd = CreateCommand(CommandType.Text, sql, query.Parameters.ToArray()))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteWhereIdIsBetween(ValueType structureIdFrom, ValueType structureIdTo, IStructureSchema structureSchema)
        {
            Ensure.That(() => structureSchema).IsNotNull();

            var sql = SqlStatements.GetSql("DeleteWhereIdIsBetween").Inject(
                structureSchema.GetIndexesTableName(),
                structureSchema.GetUniquesTableName(),
                structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(CommandType.Text, sql, new DacParameter("idFrom", structureIdFrom), new DacParameter("idTo", structureIdTo)))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public bool TableExists(string name)
        {
            Ensure.That(() => name).IsNotNullOrWhiteSpace();

            var sql = SqlStatements.GetSql("TableExists");
            var value = ExecuteScalar<string>(CommandType.Text, sql, new DacParameter("tableName", name));

            return !string.IsNullOrWhiteSpace(value);
        }

        public IList<DbColumn> GetColumns(string tableName, params string[] namesToSkip)
        {
            Ensure.That(() => tableName).IsNotNullOrWhiteSpace();

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

        public int RowCount(IStructureSchema structureSchema)
        {
            Ensure.That(() => structureSchema).IsNotNull();

            var sql = SqlStatements.GetSql("RowCount").Inject(structureSchema.GetStructureTableName());

            return ExecuteScalar<int>(CommandType.Text, sql);
        }

        public int RowCountByQuery(IStructureSchema structureSchema, SqlQuery query)
        {
            Ensure.That(() => structureSchema).IsNotNull();

            var sql = SqlStatements.GetSql("RowCountByQuery").Inject(
                structureSchema.GetIndexesTableName(),
                query.Sql);

            return ExecuteScalar<int>(CommandType.Text, sql, query.Parameters.ToArray());
        }

        public long CheckOutAndGetNextIdentity(string entityHash, int numOfIds)
        {
            Ensure.That(() => entityHash).IsNotNullOrWhiteSpace();

            var sql = SqlStatements.GetSql("Sys_Identities_CheckOutAndGetNextIdentity");

            return ExecuteScalar<long>(CommandType.Text, sql,
                new DacParameter("entityHash", entityHash),
                new DacParameter("numOfIds", numOfIds));
        }

        public IEnumerable<string> GetJson(IStructureSchema structureSchema)
        {
            Ensure.That(() => structureSchema).IsNotNull();

            var sql = SqlStatements.GetSql("GetAllById").Inject(structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(CommandType.Text, sql))
            {
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

        public string GetJsonById(ValueType structureId, IStructureSchema structureSchema)
        {
            Ensure.That(() => structureSchema).IsNotNull();

            var sql = SqlStatements.GetSql("GetById").Inject(structureSchema.GetStructureTableName());

            return ExecuteScalar<string>(CommandType.Text, sql, new DacParameter("id", structureId));
        }

        public IEnumerable<string> GetJsonByIds(IEnumerable<ValueType> ids, StructureIdTypes idType, IStructureSchema structureSchema)
        {
            Ensure.That(() => structureSchema).IsNotNull();

            var sql = SqlStatements.GetSql("GetByIds").Inject(structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(CommandType.Text, sql))
            {
                cmd.Parameters.Add(Sql2008IdsTableParam.CreateIdsTableParam(idType, ids));

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

        public IEnumerable<string> GetJsonWhereIdIsBetween(ValueType structureIdFrom, ValueType structureIdTo, IStructureSchema structureSchema)
        {
            Ensure.That(() => structureSchema).IsNotNull();

            var sql = SqlStatements.GetSql("GetJsonWhereIdIsBetween").Inject(structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(CommandType.Text, sql, new DacParameter("idFrom", structureIdFrom), new DacParameter("idTo", structureIdTo)))
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