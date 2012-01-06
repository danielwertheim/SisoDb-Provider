using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EnsureThat;
using Machine.Specifications;
using NCore;
using NCore.Expressions;
using PineCone.Structures.Schemas;
using SisoDb.DbSchema;

namespace SisoDb.Testing.Steps
{
	public static class Shoulds
	{
		public static void should_have_been_deleted_from_structures_table(this ITestDbUtils db, IStructureSchema structureSchema, object structureId)
		{
			db.RowCount(structureSchema.GetStructureTableName(), "{0} = '{1}'".Inject(StructureStorageSchema.Fields.Id.Name, structureId)).ShouldEqual(0);
		}

		public static void should_not_have_been_deleted_from_structures_table(this ITestDbUtils db, IStructureSchema structureSchema, object structureId)
		{
			db.RowCount(structureSchema.GetStructureTableName(), "{0} = '{1}'".Inject(StructureStorageSchema.Fields.Id.Name, structureId)).ShouldEqual(1);
		}

		public static void should_have_been_deleted_from_indexes_tables(this ITestDbUtils db, IStructureSchema structureSchema, object structureId)
		{
			var indexesTableNames = structureSchema.GetIndexesTableNames();
			foreach (var indexesTableName in indexesTableNames.AllTableNames)
				db.RowCount(indexesTableName, "{0} = '{1}'".Inject(IndexStorageSchema.Fields.StructureId.Name, structureId)).ShouldEqual(0);
		}

		public static void should_not_have_been_deleted_from_indexes_tables(this ITestDbUtils db, IStructureSchema structureSchema, object structureId)
		{
			var indexesTableNames = structureSchema.GetIndexesTableNames();
			var countSum = 0;
			foreach (var indexesTableName in indexesTableNames.AllTableNames)
				countSum += db.RowCount(indexesTableName, "{0} = '{1}'".Inject(IndexStorageSchema.Fields.StructureId.Name, structureId));

			countSum.ShouldBeGreaterThan(0);
		}

		public static void should_have_been_deleted_from_uniques_table(this ITestDbUtils db, IStructureSchema structureSchema, object structureId)
		{
			db.RowCount(structureSchema.GetUniquesTableName(), "{0} = '{1}'".Inject(UniqueStorageSchema.Fields.StructureId.Name, structureId)).ShouldEqual(0);
		}

		public static void should_not_have_been_deleted_from_uniques_table(this ITestDbUtils db, IStructureSchema structureSchema, object structureId)
		{
			db.RowCount(structureSchema.GetUniquesTableName(), "{0} = '{1}'".Inject(UniqueStorageSchema.Fields.StructureId.Name, structureId)).ShouldEqual(1);
		}

		public static void should_have_X_num_of_items<T>(this ISisoDatabase db, int numOfItemsLeft) where T : class
		{
			db.ReadOnce().Count<T>().ShouldEqual(numOfItemsLeft);
		}

		public static void should_have_first_and_last_item_left<T>(this ISisoDatabase db, IList<T> structures) where T : class
		{
			var structureScema = db.StructureSchemas.GetSchema<T>();

			using (var rs = db.CreateQueryEngine())
			{
				var items = rs.Query<T>().ToList();

				structureScema.IdAccessor.GetValue(items[0]).Value.ShouldEqual(
					structureScema.IdAccessor.GetValue(structures[0]).Value);

				structureScema.IdAccessor.GetValue(items[1]).Value.ShouldEqual(
					structureScema.IdAccessor.GetValue(structures[3]).Value);
			}
		}

		public static void should_have_ids<T>(this ISisoDatabase db, params object[] ids) where T : class
		{
			Ensure.That(ids, "ids").HasItems();

			using (var qe = db.CreateQueryEngine())
			{
				foreach (var id in ids)
					qe.GetById<T>(id).ShouldNotBeNull();
			}
		}

		public static void should_not_have_ids<T>(this ISisoDatabase db, params object[] ids) where T : class
		{
			Ensure.That(ids, "ids").HasItems();

			using (var qe = db.CreateQueryEngine())
			{
				foreach (var id in ids)
					qe.GetById<T>(id).ShouldBeNull();
			}
		}

		public static void should_have_identical_structures<T>(this ISisoDatabase db, params T[] structures) where T : class
		{
			Ensure.That(structures, "structures").HasItems();

			var structureSchema = db.StructureSchemas.GetSchema(typeof(T));

			using (var qe = db.CreateQueryEngine())
			{
				foreach (var structure in structures)
				{
					var id = structureSchema.IdAccessor.GetValue(structure);
					var refetched = qe.GetById<T>(id.Value);
					refetched.ShouldBeValueEqualTo(structure);
				}
			}
		}

		public static void should_have_valid_structures<T>(this ISisoDatabase db, Action<T> validationRule) where T : class
		{
			using (var rs = db.CreateQueryEngine())
			{
				foreach (var structure in rs.Query<T>().ToEnumerable())
					validationRule(structure);
			}
		}

		public static void should_have_one_structure_with_json_containing<T>(this ISisoDatabase db, params Expression<Func<T, object>>[] parts) where T : class
		{
			var structureJson = db.ReadOnce().Query<T>().ToListOfJson().SingleOrDefault();

			structureJson.ShouldNotBeEmpty();

			foreach (var part in parts.Select(GetMemberPath))
				structureJson.Contains("\"{0}\"".Inject(part)).ShouldBeTrue();
		}

		public static void should_have_one_structure_with_json_containing<T1, T2>(this ISisoDatabase db, params Expression<Func<T2, object>>[] parts)
			where T1 : class
			where T2 : class
		{
			var structureJson = db.ReadOnce().Query<T1>().ToListOfJson().SingleOrDefault();

			structureJson.ShouldNotBeEmpty();

			foreach (var part in parts.Select(GetMemberPath))
				structureJson.Contains("\"{0}\"".Inject(part)).ShouldBeTrue();
		}

		public static void should_have_one_structure_with_json_not_containing<T1, T2>(this ISisoDatabase db, params Expression<Func<T2, object>>[] parts)
			where T1 : class
			where T2 : class
		{
			var structureJson = db.ReadOnce().Query<T1>().ToListOfJson().SingleOrDefault();

			structureJson.ShouldNotBeEmpty();

			foreach (var part in parts.Select(GetMemberPath))
				structureJson.Contains("\"{0}\"".Inject(part)).ShouldBeFalse();
		}

		private static string GetMemberPath<T>(Expression<Func<T, object>> e)
		{
			return e.GetRightMostMember().ToPath();
		}
	}
}