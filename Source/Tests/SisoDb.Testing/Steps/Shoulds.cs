using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Machine.Specifications;
using NCore;
using NCore.Expressions;
using PineCone.Structures.Schemas;
using SisoDb.DbSchema;
using SisoDb.EnsureThat;

namespace SisoDb.Testing.Steps
{
	public static class Shoulds
	{
        private static readonly IDataTypeConverter DataTypeConverter = new DataTypeConverter();

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
			foreach (var indexesTableName in indexesTableNames.All)
				db.RowCount(indexesTableName, "{0} = '{1}'".Inject(IndexStorageSchema.Fields.StructureId.Name, structureId)).ShouldEqual(0);
		}

		public static void should_not_have_been_deleted_from_indexes_tables(this ITestDbUtils db, IStructureSchema structureSchema, object structureId)
		{
			var indexesTableNames = structureSchema.GetIndexesTableNames();
			var countSum = 0;
			foreach (var indexesTableName in indexesTableNames.All)
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

        public static void should_have_none_items_left<T>(this ISisoDatabase db) where T : class
        {
            db.UseOnceTo().Query<T>().Count().ShouldEqual(0);
        }

		public static void should_have_X_num_of_items<T>(this ISisoDatabase db, int numOfItemsLeft) where T : class
		{
			db.UseOnceTo().Query<T>().Count().ShouldEqual(numOfItemsLeft);
		}

		public static void should_have_first_and_last_item_left<T>(this ISisoDatabase db, IList<T> structures) where T : class
		{
			var structureScema = db.StructureSchemas.GetSchema<T>();

			using (var rs = db.BeginSession())
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

			using (var session =db.BeginSession())
			{
				foreach (var id in ids)
					session.GetById<T>(id).ShouldNotBeNull();
			}
		}

		public static void should_not_have_ids<T>(this ISisoDatabase db, params object[] ids) where T : class
		{
			Ensure.That(ids, "ids").HasItems();

			using (var session =db.BeginSession())
			{
				foreach (var id in ids)
					session.GetById<T>(id).ShouldBeNull();
			}
		}

		public static void should_have_identical_structures<T>(this ISisoDatabase db, params T[] structures) where T : class
		{
			Ensure.That(structures, "structures").HasItems();

			var structureSchema = db.StructureSchemas.GetSchema(typeof(T));

			using (var session =db.BeginSession())
			{
				foreach (var structure in structures)
				{
					var id = structureSchema.IdAccessor.GetValue(structure);
					var refetched = session.GetById<T>(id.Value);
					refetched.ShouldBeValueEqualTo(structure);
				}
			}
		}

		public static void should_have_valid_structures<T>(this ISisoDatabase db, Action<T> validationRule) where T : class
		{
			using (var rs = db.BeginSession())
			{
				foreach (var structure in rs.Query<T>().ToEnumerable())
					validationRule(structure);
			}
		}

		public static void should_have_one_structure_with_json_containing<T>(this ISisoDatabase db, params Expression<Func<T, object>>[] parts) where T : class
		{
			var structureJson = db.UseOnceTo().Query<T>().ToListOfJson().SingleOrDefault();

			structureJson.ShouldNotBeEmpty();

			foreach (var part in parts.Select(GetMemberPath))
				structureJson.Contains("\"{0}\"".Inject(part)).ShouldBeTrue();
		}

		public static void should_have_one_structure_with_json_containing<T1, T2>(this ISisoDatabase db, params Expression<Func<T2, object>>[] parts)
			where T1 : class
			where T2 : class
		{
			var structureJson = db.UseOnceTo().Query<T1>().ToListOfJson().SingleOrDefault();

			structureJson.ShouldNotBeEmpty();

			foreach (var part in parts.Select(GetMemberPath))
				structureJson.Contains("\"{0}\"".Inject(part)).ShouldBeTrue();
		}

		public static void should_have_one_structure_with_json_not_containing<T1, T2>(this ISisoDatabase db, params Expression<Func<T2, object>>[] parts)
			where T1 : class
			where T2 : class
		{
			var structureJson = db.UseOnceTo().Query<T1>().ToListOfJson().SingleOrDefault();

			structureJson.ShouldNotBeEmpty();

			foreach (var part in parts.Select(GetMemberPath))
				structureJson.Contains("\"{0}\"".Inject(part)).ShouldBeFalse();
		}

        public static void should_have_stored_member_in_indexes_table<T>(this ITestDbUtils db, IStructureSchema structureSchema, object structureId, Expression<Func<T, object>> memberExpression, Type memberType)
        {
            var memberPath = GetMemberPath(memberExpression);
            structureSchema.IndexAccessors.Count(iac => iac.Path == memberPath).ShouldBeGreaterThan(0);

            var tablename = structureSchema.GetIndexesTableNames().GetNameByType(DataTypeConverter.Convert(memberType, memberPath));
            db.RowCount(tablename, "{0} = '{1}' and {2} = '{3}'".Inject(
                IndexStorageSchema.Fields.StructureId.Name, structureId,
                IndexStorageSchema.Fields.MemberPath.Name, memberPath)).ShouldBeGreaterThan(0);
        }

        public static void should_not_have_stored_member_in_indexes_table<T>(this ITestDbUtils db, IStructureSchema structureSchema, object structureId, Expression<Func<T, object>> memberExpression, Type memberType)
        {
            var memberPath = GetMemberPath(memberExpression);
            structureSchema.IndexAccessors.Count(iac => iac.Path == memberPath).ShouldEqual(0);

            var tablename = structureSchema.GetIndexesTableNames().GetNameByType(DataTypeConverter.Convert(memberType, memberPath));
            db.RowCount(tablename, "{0} = '{1}' and {2} = '{3}'".Inject(
                IndexStorageSchema.Fields.StructureId.Name, structureId,
                IndexStorageSchema.Fields.MemberPath.Name, memberPath)).ShouldEqual(0);
        }

        public static void should_have_column(this ITestDbUtils db, string tablename, string columnName)
        {
            db.GetColumns(tablename)
                .SingleOrDefault(c => c.Name == columnName)
                .ShouldNotBeNull();
        }

        public static void should_not_have_column(this ITestDbUtils db, string tablename, string columnName)
        {
            db.GetColumns(tablename)
                .SingleOrDefault(c => c.Name == columnName)
                .ShouldBeNull();
        }

        public static void should_have_column_in_all_indexestables(this ITestDbUtils db, IndexesTableNames tableNames, string columnName)
        {
            var columnsPerTable = tableNames.All
                .GroupBy(t => t)
                .Select(t => new { Name = t.Key, HasRowId = db.GetColumns(t.Key).Any(c => c.Name == columnName) }).ToArray();

            columnsPerTable.Count(v => v.HasRowId == false).ShouldEqual(0);
        }

        public static void should_not_have_column_in_any_indexestables(this ITestDbUtils db, IndexesTableNames tableNames, string columnName)
        {
            var columnsPerTable = tableNames.All
                .GroupBy(t => t)
                .Select(t => new { Name = t.Key, HasRowId = db.GetColumns(t.Key).Any(c => c.Name == columnName) }).ToArray();

            columnsPerTable.Count(v => v.HasRowId).ShouldEqual(0);
        }

		private static string GetMemberPath<T>(Expression<Func<T, object>> e)
		{
			return e.GetRightMostMember().ToPath();
		}
	}
}