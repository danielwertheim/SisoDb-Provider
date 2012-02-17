using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EnsureThat;
using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Core;
using SisoDb.DbSchema;
using SisoDb.Querying.Sql;

namespace SisoDb.Dac
{
	public abstract class DbClientBase : ITransactionalDbClient
	{
	    protected readonly IConnectionManager ConnectionManager;
	    protected IDbConnection Connection;
	    protected readonly ISqlStatements SqlStatements;
	    
        public ISisoConnectionInfo ConnectionInfo { get; private set; }
        public ISisoTransaction Transaction { get; protected set; }

		protected DbClientBase(ISisoConnectionInfo connectionInfo, IDbConnection connection, ISisoTransaction transaction, IConnectionManager connectionManager, ISqlStatements sqlStatements)
		{
			Ensure.That(connectionInfo, "connectionInfo").IsNotNull();
            Ensure.That(connection, "connection").IsNotNull();
			Ensure.That(connectionManager, "connectionManager").IsNotNull();
			Ensure.That(sqlStatements, "sqlStatements").IsNotNull();
		    Ensure.That(transaction, "transaction").IsNotNull();

			ConnectionInfo = connectionInfo;
			ConnectionManager = connectionManager;
            Connection = connection;
            SqlStatements = sqlStatements;
		    Transaction = transaction;
		}

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            Exception ex = null;

            if (Transaction != null)
            {
                ex = Disposer.TryDispose(Transaction);
                Transaction = null;
            }

            if (Connection != null)
            {
                ConnectionManager.ReleaseClientDbConnection(Connection);
                Connection = null;
            }

            if (ex != null)
                throw ex;
        }

	    public abstract IDbBulkCopy GetBulkCopy();

	    public virtual void ExecuteNonQuery(string sql, params IDacParameter[] parameters)
		{
			Connection.ExecuteNonQuery(sql, parameters);
		}

        public virtual T ExecuteScalar<T>(string sql, params IDacParameter[] parameters)
        {
            using (var cmd = CreateCommand(sql, parameters))
            {
                return cmd.GetScalarResult<T>();
            }
        }

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

		public abstract long CheckOutAndGetNextIdentity(string entityName, int numOfIds);

        public virtual bool Exists(IStructureId structureId, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("ExistsById").Inject(structureSchema.GetStructureTableName());

            return ExecuteScalar<int>(sql, new DacParameter("id", structureId.Value)) > 0;
        }

		public virtual string GetJsonById(IStructureId structureId, IStructureSchema structureSchema)
		{
			Ensure.That(structureSchema, "structureSchema").IsNotNull();

			var sql = SqlStatements.GetSql("GetJsonById").Inject(structureSchema.GetStructureTableName());

			return ExecuteScalar<string>(sql, new DacParameter("id", structureId.Value));
		}

	    public virtual string GetJsonByIdWithLock(IStructureId structureId, IStructureSchema structureSchema)
	    {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("GetJsonByIdWithLock").Inject(structureSchema.GetStructureTableName());

            return ExecuteScalar<string>(sql, new DacParameter("id", structureId.Value));
	    }

	    public virtual IEnumerable<string> GetJsonOrderedByStructureId(IStructureSchema structureSchema)
		{
			Ensure.That(structureSchema, "structureSchema").IsNotNull();

			var sql = SqlStatements.GetSql("GetAllJson").Inject(structureSchema.GetStructureTableName());

			return YieldJson(sql);
		}

		public abstract IEnumerable<string> GetJsonByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema);

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

	    public virtual void SingleInsertStructure(IStructure structure, IStructureSchema structureSchema)
	    {
            var sql = SqlStatements.GetSql("SingleInsertStructure").Inject(
                structureSchema.GetStructureTableName(),
                StructureStorageSchema.Fields.Id.Name,
                StructureStorageSchema.Fields.Json.Name);

            ExecuteNonQuery(sql,
                new DacParameter(StructureStorageSchema.Fields.Id.Name, structure.Id.Value),
                new DacParameter(StructureStorageSchema.Fields.Json.Name, structure.Data));
	    }

	    public virtual void SingleInsertOfValueTypeIndex(IStructureIndex structureIndex, string valueTypeIndexesTableName)
	    {
            var sql = SqlStatements.GetSql("SingleInsertOfValueTypeIndex").Inject(
                valueTypeIndexesTableName,
                IndexStorageSchema.Fields.StructureId.Name,
                IndexStorageSchema.Fields.MemberPath.Name,
                IndexStorageSchema.Fields.Value.Name,
                IndexStorageSchema.Fields.StringValue.Name);

            ExecuteNonQuery(sql,
                new DacParameter(IndexStorageSchema.Fields.StructureId.Name, structureIndex.StructureId.Value),
                new DacParameter(IndexStorageSchema.Fields.MemberPath.Name, structureIndex.Path),
                new DacParameter(IndexStorageSchema.Fields.Value.Name, structureIndex.Value),
                new DacParameter(IndexStorageSchema.Fields.StringValue.Name, SisoEnvironment.StringConverter.AsString(structureIndex.Value)));
	    }

	    public virtual void SingleInsertOfStringTypeIndex(IStructureIndex structureIndex, string stringishIndexesTableName)
	    {
            var sql = SqlStatements.GetSql("SingleInsertOfStringTypeIndex").Inject(
                stringishIndexesTableName,
                IndexStorageSchema.Fields.StructureId.Name,
                IndexStorageSchema.Fields.MemberPath.Name,
                IndexStorageSchema.Fields.Value.Name);

            ExecuteNonQuery(sql,
                new DacParameter(IndexStorageSchema.Fields.StructureId.Name, structureIndex.StructureId.Value),
                new DacParameter(IndexStorageSchema.Fields.MemberPath.Name, structureIndex.Path),
                new DacParameter(IndexStorageSchema.Fields.Value.Name, structureIndex.Value.ToString()));
	    }

        public virtual void SingleInsertOfUniqueIndex(IStructureIndex uniqueStructureIndex, IStructureSchema structureSchema)
        {
            var sql = SqlStatements.GetSql("SingleInsertOfUniqueIndex").Inject(
                structureSchema.GetUniquesTableName(),
                UniqueStorageSchema.Fields.StructureId.Name,
                UniqueStorageSchema.Fields.UqStructureId.Name,
                UniqueStorageSchema.Fields.UqMemberPath.Name,
                UniqueStorageSchema.Fields.UqValue.Name);

            var parameters = new DacParameter[4];
            parameters[0] = new DacParameter(UniqueStorageSchema.Fields.StructureId.Name, uniqueStructureIndex.StructureId.Value);
            parameters[1] = (uniqueStructureIndex.IndexType == StructureIndexType.UniquePerType)
                                ? new DacParameter(UniqueStorageSchema.Fields.UqStructureId.Name, DBNull.Value)
                                : new DacParameter(UniqueStorageSchema.Fields.UqStructureId.Name, uniqueStructureIndex.StructureId.Value);
            parameters[2] = new DacParameter(UniqueStorageSchema.Fields.UqMemberPath.Name, uniqueStructureIndex.Path);
            parameters[3] = new DacParameter(UniqueStorageSchema.Fields.UqValue.Name, SisoEnvironment.HashService.GenerateHash(SisoEnvironment.StringConverter.AsString(uniqueStructureIndex.Value)));

            ExecuteNonQuery(sql, parameters);
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

        protected virtual IDbCommand CreateCommand(string sql, params IDacParameter[] parameters)
        {
            return Connection.CreateCommand(sql, parameters);
        }

        protected virtual IDbCommand CreateSpCommand(string sp, params IDacParameter[] parameters)
        {
            return Connection.CreateSpCommand(sp, parameters);
        }
	}
}