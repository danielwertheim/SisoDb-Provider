using System;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.Querying.Sql;
using SisoDb.UnitTests.TestFactories;

namespace SisoDb.UnitTests.Querying.QueryGeneration
{
	public abstract class QueryGeneratorTests : UnitTestBase
	{
		protected abstract IDbQueryGenerator GetQueryGenerator();

		public abstract void GenerateQuery_WithWhere_GeneratesCorrectQuery();

		protected SqlQuery On_GenerateQuery_WithWhere_GeneratesCorrectQuery()
		{
			var queryCommand = BuildQuery<MyClass>(q => q.Where(i => i.Int1 == 42));
			var generator = GetQueryGenerator();

			return generator.GenerateQuery(queryCommand);
		}

		public abstract void GenerateQuery_WithChainedWheres_GeneratesCorrectQuery();

		protected SqlQuery On_GenerateQuery_WithChainedWheres_GeneratesCorrectQuery()
		{
			var queryCommand = BuildQuery<MyClass>(q => q.Where(i => i.Int1 >= 40).Where(i => i.Int1 <= 42));
			var generator = GetQueryGenerator();

			return generator.GenerateQuery(queryCommand);
		}

		public abstract void GenerateQuery_WithSorting_GeneratesCorrectQuery();

		protected SqlQuery On_GenerateQuery_WithSorting_GeneratesCorrectQuery()
		{
			var queryCommand = BuildQuery<MyClass>(q => q.OrderBy(i => i.Int1));
			var generator = GetQueryGenerator();

			return generator.GenerateQuery(queryCommand);
		}

		public abstract void GenerateQuery_WithWhereAndSorting_GeneratesCorrectQuery();

		protected SqlQuery On_GenerateQuery_WithWhereAndSorting_GeneratesCorrectQuery()
		{
			var queryCommand = BuildQuery<MyClass>(q =>
			{
				q.Where(i => i.Int1 == 42);
				q.OrderBy(i => i.Int1);
			});
			var generator = GetQueryGenerator();

			return generator.GenerateQuery(queryCommand);
		}

		public abstract void GenerateQuery_WithTake_GeneratesCorrectQuery();

		protected SqlQuery On_GenerateQuery_WithTake_GeneratesCorrectQuery()
		{
			var queryCommand = BuildQuery<MyClass>(q => q.Take(11));
			var generator = GetQueryGenerator();

			return generator.GenerateQuery(queryCommand);
		}

		public abstract void GenerateQuery_WithTakeAndSorting_GeneratesCorrectQuery();

		protected SqlQuery On_GenerateQuery_WithTakeAndSorting_GeneratesCorrectQuery()
		{
			var queryCommand = BuildQuery<MyClass>(q =>
			{
				q.OrderBy(i => i.Int1);
				q.Take(11);
			});
			var generator = GetQueryGenerator();

			return generator.GenerateQuery(queryCommand);
		}

		public abstract void GenerateQuery_WithTakeAndWhereAndSorting_GeneratesCorrectQuery();

		protected SqlQuery On_GenerateQuery_WithTakeAndWhereAndSorting_GeneratesCorrectQuery()
		{
			var queryCommand = BuildQuery<MyClass>(q =>
			{
				q.Where(i => i.Int1 == 42);
				q.OrderBy(i => i.Int1);
				q.Take(11);
			});
			var generator = GetQueryGenerator();

			return generator.GenerateQuery(queryCommand);
		}

		public abstract void GenerateQuery_WithPagingAndWhereAndSorting_GeneratesCorrectQuery();

		protected SqlQuery On_GenerateQuery_WithPagingAndWhereAndSorting_GeneratesCorrectQuery()
		{
			var queryCommand = BuildQuery<MyClass>(q =>
			{
				q.Where(i => i.Int1 == 42);
				q.OrderBy(i => i.Int1);
				q.Page(0, 10);
			});
			var generator = GetQueryGenerator();

			return generator.GenerateQuery(queryCommand);
		}

		public abstract void GenerateQuery_WithExplicitSortingOnTwoDifferentMemberTypesAndSorting_GeneratesCorrectQuery();

		protected SqlQuery On_GenerateQuery_WithExplicitSortingOnTwoDifferentMemberTypesAndSorting_GeneratesCorrectQuery()
		{
			var queryCommand = BuildQuery<MyClass>(q => q.OrderBy(i => i.Int1).OrderByDescending(i => i.String1));
			var generator = GetQueryGenerator();

			return generator.GenerateQuery(queryCommand);
		}

		public abstract void GenerateQuery_WithSortingOnTwoDifferentMemberOfSameType_GeneratesCorrectQuery();

		protected SqlQuery On_GenerateQuery_WithSortingOnTwoDifferentMemberOfSameType_GeneratesCorrectQuery()
		{
			var queryCommand = BuildQuery<MyClass>(q => q.OrderBy(i => i.Int1, i => i.Int2));
			var generator = GetQueryGenerator();

			return generator.GenerateQuery(queryCommand);
		}
		
		protected virtual IQuery BuildQuery<T>(Action<IQueryBuilder<T>> commandInitializer) where T : class
		{
			var builder = new QueryBuilder<T>(new StructureSchemasStub(), new ExpressionParsers());

			commandInitializer(builder);

			return builder.Build();
		}

		private class MyClass
		{
			public int Int1 { get; set; }

			public int Int2 { get; set; }

			public string String1 { get; set; }
		}
	}
}