using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EnsureThat;
using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Core.Expressions;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Resources;

namespace SisoDb
{
	public abstract class DbUnitOfWork : DbReadSession, IUnitOfWork
	{
		protected const int MaxUpdateManyBatchSize = 1000;

		protected DbUnitOfWork(ISisoDatabase db, IDbClient dbClient, IDbSchemaManager dbSchemaManager)
			: base(db, dbClient, dbSchemaManager)
		{
		}

		public IStructureSchema GetSchema(Type type)
		{
			return Db.StructureSchemas.GetSchema(type);
		}

		public IStructureSchema GetSchema<T>() where T : class
		{
			return Db.StructureSchemas.GetSchema<T>();
		}

		public virtual void Commit()
		{
			DbClient.Flush();
		}

		public virtual void Insert<T>(T item) where T : class
		{
			var structureSchema = Db.StructureSchemas.GetSchema<T>();
			UpsertStructureSet(structureSchema);

			var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema);

			var structure = structureBuilder.CreateStructure(item, structureSchema);

			var bulkInserter = Db.ProviderFactory.GetDbStructureInserter(DbClient);
			bulkInserter.Insert(structureSchema, new[] { structure });
		}

		public virtual void InsertJson<T>(string json) where T : class
		{
			Insert(Db.Serializer.Deserialize<T>(json));
		}

		public virtual void InsertMany<T>(IList<T> items) where T : class
		{
			var structureSchema = Db.StructureSchemas.GetSchema<T>();
			UpsertStructureSet(structureSchema);

			var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema);

			var bulkInserter = Db.ProviderFactory.GetDbStructureInserter(DbClient);
			bulkInserter.Insert(structureSchema, structureBuilder.CreateStructures(items, structureSchema));
		}

		public virtual void InsertManyJson<T>(IEnumerable<string> json) where T : class
		{
			InsertMany(Db.Serializer.DeserializeMany<T>(json).ToList());
		}

		public virtual void Update<T>(T item) where T : class
		{
			var structureSchema = Db.StructureSchemas.GetSchema<T>();
			UpsertStructureSet(structureSchema);

			var structureBuilder = Db.StructureBuilders.ForUpdates(structureSchema);

			var updatedStructure = structureBuilder.CreateStructure(item, structureSchema);

			var existingItem = DbClient.GetJsonById(updatedStructure.Id, structureSchema);

			if (string.IsNullOrWhiteSpace(existingItem))
				throw new SisoDbException(ExceptionMessages.UnitOfWork_NoItemExistsForUpdate.Inject(updatedStructure.Name, updatedStructure.Id.Value));

			DeleteById(structureSchema, updatedStructure.Id);

			var bulkInserter = Db.ProviderFactory.GetDbStructureInserter(DbClient);
			bulkInserter.Insert(structureSchema, new[] { updatedStructure });
		}

		public virtual bool UpdateMany<T>(Func<T, UpdateManyModifierStatus> modifier, Expression<Func<T, bool>> expression = null) where T : class
		{
			var structureSchema = Db.StructureSchemas.GetSchema<T>();
			UpsertStructureSet(structureSchema);

			IStructureId deleteIdFrom = null, deleteIdTo = null;
			var keepQueue = new List<T>(MaxUpdateManyBatchSize);

			var structureBuilder = Db.StructureBuilders.ForUpdates(structureSchema);
			var bulkInserter = Db.ProviderFactory.GetDbStructureInserter(DbClient);

			var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas);
			if (expression != null)
			{
				queryBuilder.Where(expression);
				queryBuilder.OrderBy(ExpressionUtils.GetMemberExpression<T>(StructureSchema.IdMemberName));
			}
			var query = queryBuilder.Build();
			
			foreach (var structure in Query<T>(query))
			{
				var status = modifier.Invoke(structure);
				if (status == UpdateManyModifierStatus.Abort)
					return false;
				
				var structureId = structureSchema.IdAccessor.GetValue(structure);
				deleteIdFrom = deleteIdFrom ?? structureId;
				deleteIdTo = structureId;

				if (status == UpdateManyModifierStatus.Keep)
				{
					keepQueue.Add(structure);
					if (keepQueue.Count < MaxUpdateManyBatchSize)
						continue;

					DbClient.DeleteWhereIdIsBetween(deleteIdFrom, deleteIdTo, structureSchema);
					bulkInserter.Insert(structureSchema, structureBuilder.CreateStructures(keepQueue, structureSchema));
					keepQueue.Clear();
				}
			}

			if (keepQueue.Count > 0)
			{
				DbClient.DeleteWhereIdIsBetween(deleteIdFrom, deleteIdTo, structureSchema);
				bulkInserter.Insert(structureSchema, structureBuilder.CreateStructures(keepQueue, structureSchema));
				keepQueue.Clear();
			}

			return true;
		}

		public virtual bool UpdateMany<TOld, TNew>(Func<TOld, TNew, UpdateManyModifierStatus> modifier, Expression<Func<TOld, bool>> expression = null)
			where TOld : class
			where TNew : class
		{
			var oldType = typeof(TOld);
			var newType = typeof(TNew);
			if(oldType.Equals(newType))
				throw new SisoDbException("UpdateMany<TOld, TNew> should not be used for same types. Use UpdateMany<T> instead.");
			
			var newStructureSchema = Db.StructureSchemas.GetSchema<TNew>();
			UpsertStructureSet(newStructureSchema);

			var keepQueue = new List<TNew>(MaxUpdateManyBatchSize);

			var structureBuilder = Db.StructureBuilders.ForUpdates(newStructureSchema);
			var bulkInserter = Db.ProviderFactory.GetDbStructureInserter(DbClient);

			var queryBuilder = Db.ProviderFactory.GetQueryBuilder<TOld>(Db.StructureSchemas);
			if (expression != null)
			{
				queryBuilder.Where(expression);
				queryBuilder.OrderBy(ExpressionUtils.GetMemberExpression<TOld>(StructureSchema.IdMemberName));
			}
			var query = queryBuilder.Build();

			foreach (var oldStructureJson in QueryAsJson<TOld>(query))
			{
				var oldStructure = Db.Serializer.Deserialize<TOld>(oldStructureJson);
				var newStructure = Db.Serializer.Deserialize<TNew>(oldStructureJson);
				var status = modifier.Invoke(oldStructure, newStructure);
				if (status == UpdateManyModifierStatus.Abort)
					return false;

				keepQueue.Add(newStructure);
				if (keepQueue.Count < MaxUpdateManyBatchSize)
					continue;

				bulkInserter.Insert(newStructureSchema, structureBuilder.CreateStructures(keepQueue, newStructureSchema));
				keepQueue.Clear();
			}

			if (keepQueue.Count > 0)
			{
				bulkInserter.Insert(newStructureSchema, structureBuilder.CreateStructures(keepQueue, newStructureSchema));
				keepQueue.Clear();
			}

			if (oldType.Name.Equals(newType.Name, StringComparison.InvariantCultureIgnoreCase))
				return true;

			var oldSchema = Db.StructureSchemas.GetSchema(oldType);
			Db.StructureSchemas.RemoveSchema(oldType);
			DbSchemaManager.DropStructureSet(oldSchema, DbClient);

			return true;
		}

		public virtual void DeleteById<T>(object id) where T : class
		{
			var structureSchema = Db.StructureSchemas.GetSchema<T>();
			UpsertStructureSet(structureSchema);

			DeleteById(structureSchema, StructureId.ConvertFrom(id));
		}

		private void DeleteById(IStructureSchema structureSchema, IStructureId structureId)
		{
			DbClient.DeleteById(structureId, structureSchema);
		}

		public virtual void DeleteByIds<T>(params object[] ids) where T : class
		{
			Ensure.That(ids, "ids").HasItems();

			var structureSchema = Db.StructureSchemas.GetSchema<T>();
			UpsertStructureSet(structureSchema);

			DbClient.DeleteByIds(ids.Select(StructureId.ConvertFrom), structureSchema);
		}

		public virtual void DeleteByIdInterval<T>(object idFrom, object idTo) where T : class
		{
			var structureSchema = Db.StructureSchemas.GetSchema<T>();

			if (!structureSchema.IdAccessor.IdType.IsIdentity())
				throw new SisoDbException(ExceptionMessages.SisoDbNotSupportedByProviderException.Inject(Db.ProviderFactory.ProviderType, ExceptionMessages.UnitOfWork_DeleteByIdInterval_WrongIdType));

			UpsertStructureSet(structureSchema);

			DbClient.DeleteWhereIdIsBetween(StructureId.ConvertFrom(idFrom), StructureId.ConvertFrom(idTo), structureSchema);
		}

		public virtual void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class
		{
			Ensure.That(expression, "expression").IsNotNull();

			var structureSchema = Db.StructureSchemas.GetSchema<T>();
			UpsertStructureSet(structureSchema);

			var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas);
			queryBuilder.Where(expression);

			var sql = QueryGenerator.GenerateQueryReturningStrutureIds(queryBuilder.Build());
			DbClient.DeleteByQuery(sql, structureSchema);
		}
	}
}