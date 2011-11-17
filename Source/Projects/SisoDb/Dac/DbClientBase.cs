using System;
using System.Collections.Generic;
using System.Data;
using System.Transactions;
using EnsureThat;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Providers;
using SisoDb.Querying.Sql;
using SisoDb.Resources;

namespace SisoDb.Dac
{
    public abstract class DbClientBase : IDbClient
    {
        protected readonly ISisoProviderFactory ProviderFactory;
        protected IDbConnection Connection;
        protected IDbTransaction Transaction;
        protected TransactionScope Ts;

        public bool IsTransactional 
        {
            get { return Transaction != null || Ts != null; }
        }

        public ISqlStatements SqlStatements { get; private set; }

        protected DbClientBase(ISisoConnectionInfo connectionInfo, bool transactional)
        {
            Ensure.That(connectionInfo, "connectionInfo").IsNotNull();
            
            ProviderFactory = SisoEnvironment.ProviderFactories.Get(connectionInfo.ProviderType);
            SqlStatements = ProviderFactory.GetSqlStatements();

            Connection = ProviderFactory.GetOpenConnection(connectionInfo);

            if (System.Transactions.Transaction.Current == null)
                Transaction = transactional ? Connection.BeginTransaction() : null;
            else
                Ts = new TransactionScope(TransactionScopeOption.Suppress);
        }

        public void Dispose()
        {
            if (Transaction != null)
            {
                Transaction.Rollback();
                Transaction.Dispose();
                Transaction = null;
            }

            if (Ts != null)
            {
                Ts.Dispose();
                Ts = null;
            }

            if (Connection == null)
                return;

            ProviderFactory.ReleaseConnection(Connection);

            Connection = null;
        }

        public void Flush()
        {
            if (Ts != null)
            {
                Ts.Complete();
                Ts.Dispose();
                Ts = new TransactionScope(TransactionScopeOption.Suppress);
                return;
            }

            if (System.Transactions.Transaction.Current != null)
                return;

            if (Transaction == null)
                throw new NotSupportedException(ExceptionMessages.SqlDbClient_Flus_NonTransactional);

            Transaction.Commit();
            Transaction.Dispose();
            Transaction = Connection.BeginTransaction();
        }

        public virtual IDbCommand CreateCommand(string sql, params IDacParameter[] parameters)
        {
            return Connection.CreateCommand(Transaction, CommandType.Text, sql, parameters);
        }

        public virtual void ExecuteNonQuery(string sql, params IDacParameter[] parameters)
        {
            Connection.ExecuteNonQuery(Transaction, sql, parameters);
        }

        public virtual IDbCommand CreateSpCommand(string sp, params IDacParameter[] parameters)
        {
            return Connection.CreateCommand(Transaction, CommandType.StoredProcedure, sp, parameters);
        }

        public abstract IDbBulkCopy GetBulkCopy(bool keepIdentities);

        public abstract void Drop(IStructureSchema structureSchema);

        public abstract void RefreshIndexes(IStructureSchema structureSchema);

        public abstract void DeleteById(ValueType structureId, IStructureSchema structureSchema);

        public abstract void DeleteByIds(IEnumerable<ValueType> ids, StructureIdTypes idType, IStructureSchema structureSchema);

        public abstract void DeleteByQuery(SqlQuery query, Type idType, IStructureSchema structureSchema);

        public abstract void DeleteWhereIdIsBetween(ValueType structureIdFrom, ValueType structureIdTo, IStructureSchema structureSchema);

        public abstract bool TableExists(string name);

        public abstract int RowCount(IStructureSchema structureSchema);

        public abstract int RowCountByQuery(IStructureSchema structureSchema, SqlQuery query);

        public abstract long CheckOutAndGetNextIdentity(string entityHash, int numOfIds);

        public abstract IEnumerable<string> GetJson(IStructureSchema structureSchema);

        public abstract string GetJsonById(ValueType structureId, IStructureSchema structureSchema);

        public abstract IEnumerable<string> GetJsonByIds(IEnumerable<ValueType> ids, StructureIdTypes idType, IStructureSchema structureSchema);

        public abstract IEnumerable<string> GetJsonWhereIdIsBetween(ValueType structureIdFrom, ValueType structureIdTo, IStructureSchema structureSchema);

        protected virtual T ExecuteScalar<T>(string sql, params IDacParameter[] parameters)
        {
            using (var cmd = CreateCommand(sql, parameters))
            {
                return cmd.GetScalarResult<T>();
            }
        }

        public virtual void SingleResultSequentialReader(string sql, Action<IDataRecord> callback, params IDacParameter[] parameters)
        {
            using (var cmd = CreateCommand(sql, parameters))
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