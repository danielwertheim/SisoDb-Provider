using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EnsureThat;
using NCore;
using NCore.Collections;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Resources;
using SisoDb.Structures;

namespace SisoDb
{
	public abstract class DbWriteSession : DbReadSession, IWriteSession
	{
        protected const int MaxInsertManyBatchSize = 500;
        protected const int MaxUpdateManyBatchSize = 500;

		protected IDbClient DbClientNonTransactional;

		protected readonly IIdentityStructureIdGenerator IdentityStructureIdGenerator;

		protected DbWriteSession(
			ISisoDbDatabase db,
			IDbClient dbClientTransactional,
			IDbClient dbClientNonTransactional,
			IIdentityStructureIdGenerator identityStructureIdGenerator)
			: base(db, dbClientTransactional)
		{
			Ensure.That(dbClientNonTransactional, "dbClientNonTransactional").IsNotNull();
			Ensure.That(identityStructureIdGenerator, "identityStructureIdGenerator").IsNotNull();

			DbClientNonTransactional = dbClientNonTransactional;
			IdentityStructureIdGenerator = identityStructureIdGenerator;
		}

		public override void Dispose()
		{
			DisposeResources(true);
		}

		public virtual void DisposeWhenFailed()
		{
			DisposeResources(false);
		}

		protected virtual void DisposeResources(bool commitChanges)
		{
			GC.SuppressFinalize(this);

			if (DbClientNonTransactional == null)
				throw new ObjectDisposedException(ExceptionMessages.WriteSession_AllreadyDisposed);

			Exception exception = null;

			if (commitChanges)
			{
				try
				{
					DbClient.Commit();
				}
				catch (Exception ex)
				{
					exception = ex;
				}
			}

			base.Dispose();

			DbClientNonTransactional.Dispose();
			DbClientNonTransactional = null;

			if (exception != null)
				throw exception;
		}

		protected override void UpsertStructureSet(IStructureSchema structureSchema)
		{
			Db.SchemaManager.UpsertStructureSet(structureSchema, DbClientNonTransactional);
		}

		public virtual void Insert<T>(T item) where T : class
		{
			var structureSchema = GetStructureSchema<T>();
			UpsertStructureSet(structureSchema);

			var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema, IdentityStructureIdGenerator);

			var structure = structureBuilder.CreateStructure(item, structureSchema);

			var bulkInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
			bulkInserter.Insert(structureSchema, new[] { structure });
		}

		public virtual void InsertJson<T>(string json) where T : class
		{
			Insert(Db.Serializer.Deserialize<T>(json));
		}

        public virtual void InsertMany<T>(IEnumerable<T> items) where T : class
		{
			var structureSchema = GetStructureSchema<T>();
			UpsertStructureSet(structureSchema);

            var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema, IdentityStructureIdGenerator);

            var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);

            foreach (var structuresBatch in items.Batch(MaxInsertManyBatchSize))
                structureInserter.Insert(structureSchema, structureBuilder.CreateStructures(structuresBatch, structureSchema));
		}

		public virtual void InsertManyJson<T>(IEnumerable<string> json) where T : class
		{
			InsertMany(Db.Serializer.DeserializeMany<T>(json).ToList());
		}

		public virtual void Update<T>(T item) where T : class
		{
			var structureSchema = GetStructureSchema<T>();
			UpsertStructureSet(structureSchema);

			var structureBuilder = Db.StructureBuilders.ForUpdates(structureSchema);

			var updatedStructure = structureBuilder.CreateStructure(item, structureSchema);

			var existingItem = DbClient.GetJsonById(updatedStructure.Id, structureSchema);

			if (string.IsNullOrWhiteSpace(existingItem))
				throw new SisoDbException(ExceptionMessages.WriteSession_NoItemExistsForUpdate.Inject(updatedStructure.Name, updatedStructure.Id.Value));

			DbClient.DeleteById(updatedStructure.Id, structureSchema);

			var bulkInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
			bulkInserter.Insert(structureSchema, new[] { updatedStructure });
		}

		public virtual void UpdateMany<T>(Expression<Func<T, bool>> expression, Action<T> modifier) where T : class
		{
			Ensure.That(expression, "expression").IsNotNull();
			Ensure.That(modifier, "modifier").IsNotNull();

			var structureSchema = GetStructureSchema<T>();
			UpsertStructureSet(structureSchema);

			var deleteIds = new List<IStructureId>(MaxUpdateManyBatchSize);
			var keepQueue = new List<T>(MaxUpdateManyBatchSize);

			var structureBuilder = Db.StructureBuilders.ForUpdates(structureSchema);
			var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);

			var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(StructureSchemas);
			var query = queryBuilder.Where(expression).Build();
			var sqlQuery = QueryGenerator.GenerateQuery(query);
			foreach (var structure in Db.Serializer.DeserializeMany<T>(DbClientNonTransactional.YieldJson(sqlQuery.Sql, sqlQuery.Parameters.ToArray())))
			{
				var structureIdBefore = structureSchema.IdAccessor.GetValue(structure);
				modifier.Invoke(structure);
				var structureIdAfter = structureSchema.IdAccessor.GetValue(structure);

				if (!structureIdBefore.Value.Equals(structureIdAfter.Value))
					throw new SisoDbException(ExceptionMessages.WriteSession_UpdateMany_NewIdDoesNotMatchOldId.Inject(structureIdAfter.Value, structureIdBefore.Value));

				deleteIds.Add(structureIdBefore);

				keepQueue.Add(structure);
				if (keepQueue.Count < MaxUpdateManyBatchSize)
					continue;

				DbClient.DeleteByIds(deleteIds, structureSchema);
				deleteIds.Clear();

				structureInserter.Insert(structureSchema, structureBuilder.CreateStructures(keepQueue.ToArray(), structureSchema));
				keepQueue.Clear();
			}

			if (keepQueue.Count > 0)
			{
				DbClient.DeleteByIds(deleteIds, structureSchema);
				deleteIds.Clear();

				structureInserter.Insert(structureSchema, structureBuilder.CreateStructures(keepQueue.ToArray(), structureSchema));
				keepQueue.Clear();
			}
		}

		public virtual void DeleteById<T>(object id) where T : class
		{
			var structureSchema = GetStructureSchema<T>();
			UpsertStructureSet(structureSchema);

			DbClient.DeleteById(StructureId.ConvertFrom(id), structureSchema);
		}

		public virtual void DeleteByIds<T>(params object[] ids) where T : class
		{
			Ensure.That(ids, "ids").HasItems();

			var structureSchema = GetStructureSchema<T>();
			UpsertStructureSet(structureSchema);

			DbClient.DeleteByIds(ids.Select(StructureId.ConvertFrom), structureSchema);
		}

		public virtual void DeleteByIdInterval<T>(object idFrom, object idTo) where T : class
		{
			var structureSchema = GetStructureSchema<T>();

			if (!structureSchema.IdAccessor.IdType.IsIdentity())
				throw new SisoDbException(ExceptionMessages.SisoDbNotSupportedByProviderException.Inject(Db.ProviderFactory.ProviderType, ExceptionMessages.WriteSession_DeleteByIdInterval_WrongIdType));

			UpsertStructureSet(structureSchema);

			DbClient.DeleteWhereIdIsBetween(StructureId.ConvertFrom(idFrom), StructureId.ConvertFrom(idTo), structureSchema);
		}

		public virtual void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class
		{
			Ensure.That(expression, "expression").IsNotNull();

			var structureSchema = GetStructureSchema<T>();
			UpsertStructureSet(structureSchema);

			var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(StructureSchemas);
			queryBuilder.Where(expression);

			var sql = QueryGenerator.GenerateQueryReturningStrutureIds(queryBuilder.Build());
			DbClient.DeleteByQuery(sql, structureSchema);
		}
	}
}