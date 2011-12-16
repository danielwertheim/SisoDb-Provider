using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EnsureThat;
using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Querying;
using SisoDb.Resources;
using SisoDb.Structures;

namespace SisoDb
{
	public abstract class DbReadSession : IReadSession, IAdvancedQuerySession
	{
		protected readonly ISisoDatabase Db;
		protected IDbClient DbClient;
		protected readonly IDbSchemaManager DbSchemaManager;
		protected readonly IDbQueryGenerator QueryGenerator;

		protected DbReadSession(ISisoDatabase db, IDbClient dbClient, IDbSchemaManager dbSchemaManager)
		{
			Ensure.That(db, "db").IsNotNull();
			Ensure.That(dbClient, "dbClient").IsNotNull();
			Ensure.That(dbSchemaManager, "dbSchemaManager").IsNotNull();

			Db = db;
			DbClient = dbClient;
			DbSchemaManager = dbSchemaManager;
			QueryGenerator = Db.ProviderFactory.GetDbQueryGenerator();
		}

		public virtual void Dispose()
		{
			if (DbClient != null)
			{
				DbClient.Dispose();
				DbClient = null;
			}

			GC.SuppressFinalize(this);
		}

		protected virtual void UpsertStructureSet(IStructureSchema structureSchema)
		{
			DbSchemaManager.UpsertStructureSet(structureSchema, DbClient);
		}

		public IStructureSchemas StructureSchemas
		{
			get { return Db.StructureSchemas; }
		}

		public virtual IAdvancedQuerySession Advanced
		{
			get { return this; }
		}

		IEnumerable<T> IAdvancedQuerySession.NamedQuery<T>(INamedQuery query)
		{
			return Db.Serializer.DeserializeMany<T>(((IAdvancedQuerySession)this).NamedQueryAsJson<T>(query));
		}

		IEnumerable<TOut> IAdvancedQuerySession.NamedQueryAs<TContract, TOut>(INamedQuery query)
		{
			return Db.Serializer.DeserializeMany<TOut>(((IAdvancedQuerySession)this).NamedQueryAsJson<TContract>(query));
		}

		IEnumerable<string> IAdvancedQuerySession.NamedQueryAsJson<T>(INamedQuery query)
		{
			var structureSchema = Db.StructureSchemas.GetSchema<T>();
			UpsertStructureSet(structureSchema);

			return ConsumeReader(query.Name, true, query.Parameters.ToArray());
		}

		public virtual int Count<T>(IQuery query) where T : class
		{
			Ensure.That(query, "query").IsNotNull();

			if (!query.HasWhere)
				return Count<T>();
			
			var structureSchema = Db.StructureSchemas.GetSchema<T>();
			UpsertStructureSet(structureSchema);

			var whereSql = QueryGenerator.GenerateQueryReturningStrutureIds(query);

			return DbClient.RowCountByQuery(structureSchema, whereSql);
		}

		private int Count<T>() where T : class
		{
			var structureSchema = Db.StructureSchemas.GetSchema<T>();
			UpsertStructureSet(structureSchema);

			return DbClient.RowCount(structureSchema);
		}

		public virtual T GetById<T>(object id) where T : class
		{
			return Db.Serializer.Deserialize<T>(GetByIdAsJson<T>(id));
		}

		public virtual IEnumerable<T> GetByIds<T>(params object[] ids) where T : class
		{
			return Db.Serializer.DeserializeMany<T>(GetByIdsAsJson<T>(ids));
		}

		public virtual IEnumerable<T> GetByIdInterval<T>(object idFrom, object idTo) where T : class
		{
			var structureSchema = Db.StructureSchemas.GetSchema<T>();

			if (!structureSchema.IdAccessor.IdType.IsIdentity())
				throw new SisoDbException(ExceptionMessages.SisoDbNotSupportedByProviderException.Inject(Db.ProviderFactory.ProviderType, ExceptionMessages.QuerySession_GetByIdInterval_WrongIdType));

			UpsertStructureSet(structureSchema);

			return Db.Serializer.DeserializeMany<T>(DbClient.GetJsonWhereIdIsBetween(StructureId.ConvertFrom(idFrom), StructureId.ConvertFrom(idTo), structureSchema));
		}

		public virtual TOut GetByIdAs<TContract, TOut>(object id)
			where TContract : class
			where TOut : class
		{
			return Db.Serializer.Deserialize<TOut>(GetByIdAsJson<TContract>(id));
		}

		public virtual IEnumerable<TOut> GetByIdsAs<TContract, TOut>(params object[] ids)
			where TContract : class
			where TOut : class
		{
			return Db.Serializer.DeserializeMany<TOut>(GetByIdsAsJson<TContract>(ids.Select(i => i).ToArray()));
		}

		public virtual string GetByIdAsJson<T>(object id) where T : class
		{
			var structureSchema = Db.StructureSchemas.GetSchema<T>();
			UpsertStructureSet(structureSchema);

			return DbClient.GetJsonById(StructureId.ConvertFrom(id), structureSchema);
		}

		public virtual IEnumerable<string> GetByIdsAsJson<T>(params object[] ids) where T : class
		{
			var structureSchema = Db.StructureSchemas.GetSchema<T>();
			UpsertStructureSet(structureSchema);

			return DbClient.GetJsonByIds(ids.Select(StructureId.ConvertFrom), structureSchema);
		}

		public virtual ISisoQueryable<T> Query<T>() where T : class
		{
			return new SisoQueryable<T>(Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas), this);
		}

		public virtual IEnumerable<T> Query<T>(IQuery query) where T : class
		{
			return Db.Serializer.DeserializeMany<T>(QueryAsJson<T>(query));
		}

		public IEnumerable<TResult> QueryAs<T, TResult>(IQuery query) where T : class where TResult : class
		{
			return Db.Serializer.DeserializeMany<TResult>(QueryAsJson<T>(query));
		}

		public IEnumerable<string> QueryAsJson<T>(IQuery query) where T : class
		{
			Ensure.That(query, "query").IsNotNull();

			var structureSchema = Db.StructureSchemas.GetSchema<T>();
			UpsertStructureSet(structureSchema);

			if(query.IsEmpty)
			{
				var sql = DbClient.SqlStatements.GetSql("GetAllJson").Inject(structureSchema.GetStructureTableName());
				return ConsumeReader(sql, false);
			}

			var sqlQuery = QueryGenerator.GenerateQuery(query);
			var parameters = sqlQuery.Parameters.Select(p => new DacParameter(p.Name, p.Value)).ToArray();

			return ConsumeReader(sqlQuery.Sql, false, parameters);
		}

		private IEnumerable<string> ConsumeReader(string sql, bool isStoredProcedure, params IDacParameter[] parameters)
		{
			using (var cmd = !isStoredProcedure ? DbClient.CreateCommand(sql, parameters) : DbClient.CreateSpCommand(sql, parameters))
			{
				using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess))
				{
					Func<IDataRecord, IDictionary<int, string>, string> read = (dr, af) => dr.GetString(0);
					IDictionary<int, string> additionalJsonFields = null;

					if (reader.FieldCount > 1)
					{
						additionalJsonFields = GetAdditionalJsonFields(reader);
						if (additionalJsonFields.Count > 0)
							read = GetMergedJsonStructure;
					}

					while (reader.Read())
					{
						yield return read.Invoke(reader, additionalJsonFields);
					}

					reader.Close();
				}
			}
		}

		private IDictionary<int, string> GetAdditionalJsonFields(IDataRecord dataRecord)
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