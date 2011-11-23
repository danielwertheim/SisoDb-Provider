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

        public abstract void GenerateQuery_for_Where_with_String_StartsWith_GeneratesCorrectQuery();

        protected SqlQuery On_GenerateQuery_for_Where_with_String_StartsWith_GeneratesCorrectQuery()
        {
            var queryCommand = GetQueryCommand<MyClass>(q => q.Where(i => i.String1.StartsWith("Foo")));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_for_Where_with_String_QxStartsWith_GeneratesCorrectQuery();

        protected SqlQuery On_GenerateQuery_for_Where_with_String_QxStartsWith_GeneratesCorrectQuery()
        {
            var queryCommand = GetQueryCommand<MyClass>(q => q.Where(i => i.String1.QxStartsWith("Foo")));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_for_Where_with_String_EndsWith_GeneratesCorrectQuery();

        protected SqlQuery On_GenerateQuery_for_Where_with_String_EndsWith_GeneratesCorrectQuery()
        {
            var queryCommand = GetQueryCommand<MyClass>(q => q.Where(i => i.String1.EndsWith("bar")));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_for_Where_with_String_QxEndsWith_GeneratesCorrectQuery();

        protected SqlQuery On_GenerateQuery_for_Where_with_String_QxEndsWith_GeneratesCorrectQuery()
        {
            var queryCommand = GetQueryCommand<MyClass>(q => q.Where(i => i.String1.QxEndsWith("bar")));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_for_Where_with_String_QxContains_GeneratesCorrectQuery();

        protected SqlQuery On_GenerateQuery_for_Where_with_String_QxContains_GeneratesCorrectQuery()
        {
            var queryCommand = GetQueryCommand<MyClass>(q => q.Where(i => i.String1.QxContains("bar")));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_for_Where_with_String_QxLike_GeneratesCorrectQuery();

        protected SqlQuery On_GenerateQuery_for_Where_with_String_QxLike_GeneratesCorrectQuery()
        {
            var queryCommand = GetQueryCommand<MyClass>(q => q.Where(i => i.String1.QxLike("Foo%bar")));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_for_Where_with_String_ToLower_GeneratesCorrectQuery();

        protected SqlQuery On_GenerateQuery_for_Where_with_String_ToLower_GeneratesCorrectQuery()
        {
            var queryCommand = GetQueryCommand<MyClass>(q => q.Where(i => i.String1.ToLower() == "foo"));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_for_Where_with_String_QxToLower_GeneratesCorrectQuery();

        protected SqlQuery On_GenerateQuery_for_Where_with_String_QxToLower_GeneratesCorrectQuery()
        {
            var queryCommand = GetQueryCommand<MyClass>(q => q.Where(i => i.String1.QxToLower() == "foo"));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_for_Where_with_String_ToUpper_GeneratesCorrectQuery();

        protected SqlQuery On_GenerateQuery_for_Where_with_String_ToUpper_GeneratesCorrectQuery()
        {
            var queryCommand = GetQueryCommand<MyClass>(q => q.Where(i => i.String1.ToUpper() == "FOO"));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_for_Where_with_String_QxToUpper_GeneratesCorrectQuery();

        protected SqlQuery On_GenerateQuery_for_Where_with_String_QxToUpper_GeneratesCorrectQuery()
        {
            var queryCommand = GetQueryCommand<MyClass>(q => q.Where(i => i.String1.QxToUpper() == "FOO"));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        private static IQueryCommand GetQueryCommand<T>(Action<IQueryCommandBuilder<T>> commandInitializer) where T : class
        {
            var schema = StructureSchemaTestFactory.Stub<T>();
            var builder = new QueryCommandBuilder<T>(schema, new WhereParser(), new SortingParser(), new IncludeParser());

            commandInitializer(builder);

            return builder.Command;
        }

        private class MyClass
        {
            public string String1 { get; set; }
        }
    }
}