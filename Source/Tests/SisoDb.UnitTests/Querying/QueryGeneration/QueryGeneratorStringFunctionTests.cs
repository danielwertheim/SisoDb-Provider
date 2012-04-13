using System;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.Querying.Sql;
using SisoDb.UnitTests.TestFactories;

namespace SisoDb.UnitTests.Querying.QueryGeneration
{
    public abstract class QueryGeneratorStringFunctionTests : UnitTestBase
    {
        protected abstract IDbQueryGenerator GetQueryGenerator();

		public abstract void GenerateQuery_for_Where_with_String_StartsWith_on_Nullable_ToString_GeneratesCorrectQuery();

		protected IDbQuery On_GenerateQuery_for_Where_with_String_StartsWith_on_Nullable_ToString_GeneratesCorrectQuery()
		{
			var queryCommand = BuildQuery<MyClass>(q => q.Where(i => i.NullableInt1.Value.ToString().StartsWith("42")));
			var generator = GetQueryGenerator();

			return generator.GenerateQuery(queryCommand);
		}

		public abstract void GenerateQuery_for_Where_with_String_Equals_and_StartsWith_on_String_GeneratesCorrectQuery();

        protected IDbQuery On_GenerateQuery_for_Where_with_String_Equals_and_StartsWith_on_String_GeneratesCorrectQuery()
		{
			var queryCommand = BuildQuery<MyClass>(q => q.Where(i => i.String1 == "Foo" || i.String1.StartsWith("42")));
			var generator = GetQueryGenerator();

			return generator.GenerateQuery(queryCommand);
		}

		public abstract void GenerateQuery_for_Where_with_String_EndsWith_on_Nullable_ToString_GeneratesCorrectQuery();

        protected IDbQuery On_GenerateQuery_for_Where_with_String_EndsWith_on_Nullable_ToString_GeneratesCorrectQuery()
		{
			var queryCommand = BuildQuery<MyClass>(q => q.Where(i => i.NullableInt1.Value.ToString().EndsWith("42")));
			var generator = GetQueryGenerator();

			return generator.GenerateQuery(queryCommand);
		}

		public abstract void GenerateQuery_for_Where_with_String_Equals_and_EndsWith_on_String_GeneratesCorrectQuery();

        protected IDbQuery On_GenerateQuery_for_Where_with_String_Equals_and_EndsWith_on_String_GeneratesCorrectQuery()
		{
			var queryCommand = BuildQuery<MyClass>(q => q.Where(i => i.String1 == "Foo" || i.String1.EndsWith("42")));
			var generator = GetQueryGenerator();

			return generator.GenerateQuery(queryCommand);
		}

        public abstract void GenerateQuery_for_Where_with_String_StartsWith_GeneratesCorrectQuery();

        protected IDbQuery On_GenerateQuery_for_Where_with_String_StartsWith_GeneratesCorrectQuery()
        {
            var queryCommand = BuildQuery<MyClass>(q => q.Where(i => i.String1.StartsWith("Foo")));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_for_Where_with_String_QxStartsWith_GeneratesCorrectQuery();

        protected IDbQuery On_GenerateQuery_for_Where_with_String_QxStartsWith_GeneratesCorrectQuery()
        {
            var queryCommand = BuildQuery<MyClass>(q => q.Where(i => i.String1.QxStartsWith("Foo")));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_for_Where_with_String_EndsWith_GeneratesCorrectQuery();

        protected IDbQuery On_GenerateQuery_for_Where_with_String_EndsWith_GeneratesCorrectQuery()
        {
            var queryCommand = BuildQuery<MyClass>(q => q.Where(i => i.String1.EndsWith("bar")));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_for_Where_with_String_QxEndsWith_GeneratesCorrectQuery();

        protected IDbQuery On_GenerateQuery_for_Where_with_String_QxEndsWith_GeneratesCorrectQuery()
        {
            var queryCommand = BuildQuery<MyClass>(q => q.Where(i => i.String1.QxEndsWith("bar")));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_for_Where_with_String_QxContains_GeneratesCorrectQuery();

        protected IDbQuery On_GenerateQuery_for_Where_with_String_QxContains_GeneratesCorrectQuery()
        {
            var queryCommand = BuildQuery<MyClass>(q => q.Where(i => i.String1.QxContains("bar")));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_for_Where_with_String_Contains_GeneratesCorrectQuery();

        protected IDbQuery On_GenerateQuery_for_Where_with_String_Contains_GeneratesCorrectQuery()
        {
            var queryCommand = BuildQuery<MyClass>(q => q.Where(i => i.String1.Contains("bar")));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_for_Where_with_String_QxLike_GeneratesCorrectQuery();

        protected IDbQuery On_GenerateQuery_for_Where_with_String_QxLike_GeneratesCorrectQuery()
        {
            var queryCommand = BuildQuery<MyClass>(q => q.Where(i => i.String1.QxLike("Foo%bar")));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_for_Where_with_String_ToLower_GeneratesCorrectQuery();

        protected IDbQuery On_GenerateQuery_for_Where_with_String_ToLower_GeneratesCorrectQuery()
        {
            var queryCommand = BuildQuery<MyClass>(q => q.Where(i => i.String1.ToLower() == "foo"));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_for_Where_with_String_QxToLower_GeneratesCorrectQuery();

        protected IDbQuery On_GenerateQuery_for_Where_with_String_QxToLower_GeneratesCorrectQuery()
        {
            var queryCommand = BuildQuery<MyClass>(q => q.Where(i => i.String1.QxToLower() == "foo"));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_for_Where_with_String_ToUpper_GeneratesCorrectQuery();

        protected IDbQuery On_GenerateQuery_for_Where_with_String_ToUpper_GeneratesCorrectQuery()
        {
            var queryCommand = BuildQuery<MyClass>(q => q.Where(i => i.String1.ToUpper() == "FOO"));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_for_Where_with_String_QxToUpper_GeneratesCorrectQuery();

        protected IDbQuery On_GenerateQuery_for_Where_with_String_QxToUpper_GeneratesCorrectQuery()
        {
            var queryCommand = BuildQuery<MyClass>(q => q.Where(i => i.String1.QxToUpper() == "FOO"));
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
            public string String1 { get; set; }

			public int? NullableInt1 { get; set; }
        }
    }
}