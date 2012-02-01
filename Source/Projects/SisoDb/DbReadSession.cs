using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Core;
using SisoDb.Dac;
using SisoDb.Querying;
using SisoDb.Resources;

namespace SisoDb
{
	public abstract class DbReadSession : IReadSession, IQueryEngine, IAdvancedQueries
	{
		protected readonly ISisoDbDatabase Db;
		protected readonly IDbQueryGenerator QueryGenerator;
		protected readonly ISqlStatements SqlStatements;
		protected IDbClient DbClient;

		protected DbReadSession(ISisoDbDatabase db, IDbClient dbClient)
		{
			Ensure.That(db, "db").IsNotNull();
			Ensure.That(dbClient, "dbClient").IsNotNull();
			
			Db = db;
			DbClient = dbClient;
			SqlStatements = Db.ProviderFactory.GetSqlStatements();
			QueryGenerator = Db.ProviderFactory.GetDbQueryGenerator();
		}

		public virtual void Dispose()
		{
			GC.SuppressFinalize(this);

			if (DbClient == null)
				throw new ObjectDisposedException(ExceptionMessages.ReadSession_AllreadyDisposed);

			if (DbClient != null)
			{
				DbClient.Dispose();
				DbClient = null;
			}
		}

		protected virtual void UpsertStructureSet(IStructureSchema structureSchema)
		{
			Db.SchemaManager.UpsertStructureSet(structureSchema, DbClient);
		}

		protected IStructureSchemas StructureSchemas
		{
			get { return Db.StructureSchemas; }
		}

		public virtual IQueryEngine QueryEngine
		{
			get { return this; }
		}

		public virtual IAdvancedQueries Advanced
		{
			get { return this; }
		}

		public IStructureSchema GetStructureSchema<T>() where T : class
		{
			return StructureSchemas.GetSchema<T>();
		}

		IEnumerable<T> IAdvancedQueries.NamedQuery<T>(INamedQuery query)
		{
            Ensure.That(query, "query").IsNotNull();

			return Db.Serializer.DeserializeMany<T>(((IAdvancedQueries)this).NamedQueryAsJson<T>(query));
		}

		IEnumerable<TOut> IAdvancedQueries.NamedQueryAs<TContract, TOut>(INamedQuery query)
		{
            Ensure.That(query, "query").IsNotNull();

			return Db.Serializer.DeserializeMany<TOut>(((IAdvancedQueries)this).NamedQueryAsJson<TContract>(query));
		}

		IEnumerable<string> IAdvancedQueries.NamedQueryAsJson<T>(INamedQuery query)
		{
            Ensure.That(query, "query").IsNotNull();

			var structureSchema = GetStructureSchema<T>();
			UpsertStructureSet(structureSchema);

			return DbClient.YieldJsonBySp(query.Name, query.Parameters.ToArray());
		}

		IEnumerable<T> IAdvancedQueries.RawQuery<T>(IRawQuery query)
		{
            Ensure.That(query, "query").IsNotNull();

			return Db.Serializer.DeserializeMany<T>(((IAdvancedQueries)this).RawQueryAsJson<T>(query));
		}

		IEnumerable<TOut> IAdvancedQueries.RawQueryAs<TContract, TOut>(IRawQuery query)
		{
            Ensure.That(query, "query").IsNotNull();

			return Db.Serializer.DeserializeMany<TOut>(((IAdvancedQueries)this).RawQueryAsJson<TContract>(query));
		}

		IEnumerable<string> IAdvancedQueries.RawQueryAsJson<T>(IRawQuery query)
		{
            Ensure.That(query, "query").IsNotNull();

			var structureSchema = GetStructureSchema<T>();
			UpsertStructureSet(structureSchema);

			return DbClient.YieldJson(query.QueryString, query.Parameters.ToArray());
		}

		public virtual int Count<T>(IQuery query) where T : class
		{
			Ensure.That(query, "query").IsNotNull();

			if (!query.HasWhere)
				return Count<T>();

			var structureSchema = GetStructureSchema<T>();
			UpsertStructureSet(structureSchema);

			var whereSql = QueryGenerator.GenerateQueryReturningStrutureIds(query);

			return DbClient.RowCountByQuery(structureSchema, whereSql);
		}

		private int Count<T>() where T : class
		{
			var structureSchema = GetStructureSchema<T>();
			UpsertStructureSet(structureSchema);

			return DbClient.RowCount(structureSchema);
		}

		public virtual T GetById<T>(object id) where T : class
		{
            Ensure.That(id, "id").IsNotNull();

			return Db.Serializer.Deserialize<T>(GetByIdAsJson<T>(id));
		}

		public virtual IEnumerable<T> GetByIds<T>(params object[] ids) where T : class
		{
            Ensure.That(ids, "ids").HasItems();

			return Db.Serializer.DeserializeMany<T>(GetByIdsAsJson<T>(ids));
		}

		public virtual IEnumerable<T> GetByIdInterval<T>(object idFrom, object idTo) where T : class
		{
            Ensure.That(idFrom, "idFrom").IsNotNull();
            Ensure.That(idTo, "idTo").IsNotNull();

			var structureSchema = GetStructureSchema<T>();

			if (!structureSchema.IdAccessor.IdType.IsIdentity())
				throw new SisoDbException(ExceptionMessages.SisoDbNotSupportedByProviderException.Inject(Db.ProviderFactory.ProviderType, ExceptionMessages.ReadSession_GetByIdInterval_WrongIdType));

			UpsertStructureSet(structureSchema);

			return Db.Serializer.DeserializeMany<T>(DbClient.GetJsonWhereIdIsBetween(StructureId.ConvertFrom(idFrom), StructureId.ConvertFrom(idTo), structureSchema));
		}

		public virtual TOut GetByIdAs<TContract, TOut>(object id)
			where TContract : class
			where TOut : class
		{
            Ensure.That(id, "id").IsNotNull();

			return Db.Serializer.Deserialize<TOut>(GetByIdAsJson<TContract>(id));
		}

		public virtual IEnumerable<TOut> GetByIdsAs<TContract, TOut>(params object[] ids)
			where TContract : class
			where TOut : class
		{
            Ensure.That(ids, "ids").HasItems();

			return Db.Serializer.DeserializeMany<TOut>(GetByIdsAsJson<TContract>(ids.Select(i => i).ToArray()));
		}

		public virtual string GetByIdAsJson<T>(object id) where T : class
		{
            Ensure.That(id, "id").IsNotNull();

			var structureSchema = GetStructureSchema<T>();
			UpsertStructureSet(structureSchema);

			return DbClient.GetJsonById(StructureId.ConvertFrom(id), structureSchema);
		}

		public virtual IEnumerable<string> GetByIdsAsJson<T>(params object[] ids) where T : class
		{
            Ensure.That(ids, "ids").HasItems();

			var structureSchema = GetStructureSchema<T>();
			UpsertStructureSet(structureSchema);

			return DbClient.GetJsonByIds(ids.Yield().Select(StructureId.ConvertFrom), structureSchema);
		}

		public virtual ISisoQueryable<T> Query<T>() where T : class
		{
			return new SisoQueryable<T>(Db.ProviderFactory.GetQueryBuilder<T>(StructureSchemas), this);
		}

		public virtual IEnumerable<T> Query<T>(IQuery query) where T : class
		{
            Ensure.That(query, "query").IsNotNull();

			return Db.Serializer.DeserializeMany<T>(QueryAsJson<T>(query));
		}

		public IEnumerable<TResult> QueryAs<T, TResult>(IQuery query) where T : class where TResult : class
		{
            Ensure.That(query, "query").IsNotNull();

			return Db.Serializer.DeserializeMany<TResult>(QueryAsJson<T>(query));
		}

		public IEnumerable<string> QueryAsJson<T>(IQuery query) where T : class
		{
			Ensure.That(query, "query").IsNotNull();

			var structureSchema = GetStructureSchema<T>();
			UpsertStructureSet(structureSchema);

			if (query.IsEmpty)
				return DbClient.GetJsonOrderedByStructureId(structureSchema);

			var sqlQuery = QueryGenerator.GenerateQuery(query);

			return DbClient.YieldJson(sqlQuery.Sql, sqlQuery.Parameters.ToArray());
		}
	}
}