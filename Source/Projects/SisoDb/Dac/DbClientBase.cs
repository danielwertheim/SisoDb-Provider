using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;
using EnsureThat;
using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.DbSchema;
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

		protected virtual IDbCommand CreateCommand(string sql, params IDacParameter[] parameters)
		{
			return Connection.CreateCommand(Transaction, sql, parameters);
		}

		protected virtual IDbCommand CreateSpCommand(string sp, params IDacParameter[] parameters)
		{
			return Connection.CreateSpCommand(Transaction, sp, parameters);
		}

		public virtual void ExecuteNonQuery(string sql, params IDacParameter[] parameters)
		{
			Connection.ExecuteNonQuery(Transaction, sql, parameters);
		}

		public abstract IDbBulkCopy GetBulkCopy();

		public abstract void Drop(IStructureSchema structureSchema);

		public abstract void DeleteById(IStructureId structureId, IStructureSchema structureSchema);

		public abstract void DeleteByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema);

		public abstract void DeleteByQuery(DbQuery query, IStructureSchema structureSchema);

		public abstract void DeleteWhereIdIsBetween(IStructureId structureIdFrom, IStructureId structureIdTo, IStructureSchema structureSchema);

		public abstract bool TableExists(string name);

		public virtual IndexesTableStatuses GetIndexesTableStatuses(IndexesTableNames names)
		{
			return new IndexesTableStatuses(names)
			{
				IntegersTableExists = TableExists(names.IntegersTableName),
				FractalsTableExists = TableExists(names.FractalsTableName),
				DatesTableExists = TableExists(names.DatesTableName),
				BooleansTableExists = TableExists(names.BooleansTableName),
				GuidsTableExists = TableExists(names.GuidsTableName),
				StringsTableExists = TableExists(names.StringsTableName),
				TextsTableExists = TableExists(names.TextsTableName)
			};
		}

		public abstract int RowCount(IStructureSchema structureSchema);

		public abstract int RowCountByQuery(IStructureSchema structureSchema, DbQuery query);

		public abstract long CheckOutAndGetNextIdentity(string entityHash, int numOfIds);

		public virtual string GetJsonById(IStructureId structureId, IStructureSchema structureSchema)
		{
			Ensure.That(structureSchema, "structureSchema").IsNotNull();

			var sql = SqlStatements.GetSql("GetJsonById").Inject(structureSchema.GetStructureTableName());

			return ExecuteScalar<string>(sql, new DacParameter("id", structureId.Value));
		}

		public abstract IEnumerable<string> GetJsonByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema);

		public virtual IEnumerable<string> GetJsonWhereIdIsBetween(IStructureId structureIdFrom, IStructureId structureIdTo, IStructureSchema structureSchema)
		{
			Ensure.That(structureSchema, "structureSchema").IsNotNull();

			var sql = SqlStatements.GetSql("GetJsonWhereIdIsBetween").Inject(structureSchema.GetStructureTableName());

			return YieldJson(sql, new DacParameter("idFrom", structureIdFrom.Value), new DacParameter("idTo", structureIdTo.Value));
		}

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

		public virtual IEnumerable<string> YieldJson(string sql, params IDacParameter[] parameters)
		{
			using (var cmd = CreateCommand(sql, parameters))
			{
				foreach (var json in YieldJson(cmd))
					yield return json;
			}
		}

		public virtual IEnumerable<string> YieldJsonBySp(string sql, params IDacParameter[] parameters)
		{
			using (var cmd = CreateSpCommand(sql, parameters))
			{
				foreach (var json in YieldJson(cmd))
					yield return json;
			}
		}

		private IEnumerable<string> YieldJson(IDbCommand cmd)
		{
			Func<IDataRecord, IDictionary<int, string>, string> read = (dr, af) => dr.GetString(0);
			IDictionary<int, string> additionalJsonFields = null;

			using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess))
			{
				if (reader.Read())
				{
					if (reader.FieldCount > 1)
					{
						additionalJsonFields = GetAdditionalJsonFields(reader);
						if (additionalJsonFields.Count > 0)
							read = GetMergedJsonStructure;
					}
					yield return read.Invoke(reader, additionalJsonFields);
				}

				while (reader.Read())
				{
					yield return read.Invoke(reader, additionalJsonFields);
				}
				reader.Close();
			}
		}

		private static IDictionary<int, string> GetAdditionalJsonFields(IDataRecord dataRecord)
		{
			var indices = new Dictionary<int, string>();
			for (var i = 1; i < dataRecord.FieldCount; i++)
			{
				var name = dataRecord.GetName(i);
				if (name.Contains(StructureStorageSchema.Fields.Json.Name))
					indices.Add(i, name);
				else
					break;
			}
			return indices;
		}

		private static string GetMergedJsonStructure(IDataRecord dataRecord, IDictionary<int, string> additionalJsonFields)
		{
			var sb = new StringBuilder();
			sb.Append(dataRecord.GetString(0));
			sb = sb.Remove(sb.Length - 1, 1);

			foreach (var childJson in ReadChildJson(dataRecord, additionalJsonFields))
			{
				sb.Append(",");
				sb.Append(childJson);
			}

			sb.Append("}");

			return sb.ToString();
		}

		private static IEnumerable<string> ReadChildJson(IDataRecord dataRecord, IEnumerable<KeyValuePair<int, string>> additionalJsonFields)
		{
			return additionalJsonFields.Select(additionalJsonField =>
				string.Format("\"{0}\":{1}",
				additionalJsonField.Value.Replace(StructureStorageSchema.Fields.Json.Name, string.Empty),
				dataRecord.GetString(additionalJsonField.Key)));
		}
	}
}