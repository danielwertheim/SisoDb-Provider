using System;
using System.Collections.Generic;
using System.Data;
using System.Transactions;
using EnsureThat;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Querying.Sql;
using SisoDb.Resources;

namespace SisoDb.Dac
{
    public abstract class DbClientBase : IDbClient
    {
    	protected readonly ISisoConnectionInfo ConnectionInfo;
    	protected readonly IConnectionManager ConnectionManager;
        protected IDbConnection Connection;
        protected IDbTransaction Transaction;
        protected TransactionScope Ts;
		protected readonly ISqlStatements SqlStatements;

        public bool IsTransactional 
        {
            get { return Transaction != null || Ts != null; }
        }
		
        protected DbClientBase(ISisoConnectionInfo connectionInfo, bool transactional, IConnectionManager connectionManager, ISqlStatements sqlStatements)
        {
            Ensure.That(connectionInfo, "connectionInfo").IsNotNull();
			Ensure.That(connectionManager, "connectionManager").IsNotNull();
			Ensure.That(sqlStatements, "sqlStatements").IsNotNull();

        	ConnectionInfo = connectionInfo;
        	ConnectionManager = connectionManager;
        	SqlStatements = sqlStatements;

            Connection = ConnectionManager.OpenDbConnection(connectionInfo.ConnectionString);

            if (System.Transactions.Transaction.Current == null)
                Transaction = transactional ? Connection.BeginTransaction() : null;
            else
                Ts = new TransactionScope(TransactionScopeOption.Suppress);
        }

        public void Dispose()
        {
			GC.SuppressFinalize(this);

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

            ConnectionManager.ReleaseDbConnection(Connection);

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

        public abstract IDbBulkCopy GetBulkCopy();

        public abstract void Drop(IStructureSchema structureSchema);

        public abstract void RefreshIndexes(IStructureSchema structureSchema);

        public abstract void DeleteById(IStructureId structureId, IStructureSchema structureSchema);

        public abstract void DeleteByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema);

        public abstract void DeleteByQuery(SqlQuery query, IStructureSchema structureSchema);

        public abstract void DeleteWhereIdIsBetween(IStructureId structureIdFrom, IStructureId structureIdTo, IStructureSchema structureSchema);

        public abstract bool TableExists(string name);

        public abstract int RowCount(IStructureSchema structureSchema);

        public abstract int RowCountByQuery(IStructureSchema structureSchema, SqlQuery query);

        public abstract long CheckOutAndGetNextIdentity(string entityHash, int numOfIds);

        public abstract string GetJsonById(IStructureId structureId, IStructureSchema structureSchema);

        public abstract IEnumerable<string> GetJsonByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema);

        public abstract IEnumerable<string> GetJsonWhereIdIsBetween(IStructureId structureIdFrom, IStructureId structureIdTo, IStructureSchema structureSchema);

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