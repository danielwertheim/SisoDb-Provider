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
            var queryCommand = GetQueryCommand<MyClass>(q => q.Where(i => i.Int1 == 42));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_WithSorting_GeneratesCorrectQuery();

        protected SqlQuery On_GenerateQuery_WithSorting_GeneratesCorrectQuery()
        {
            var queryCommand = GetQueryCommand<MyClass>(q => q.SortBy(i => i.Int1));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_WithWhereAndSorting_GeneratesCorrectQuery();

        protected SqlQuery On_GenerateQuery_WithWhereAndSorting_GeneratesCorrectQuery()
        {
            var queryCommand = GetQueryCommand<MyClass>(q =>
            {
                q.Where(i => i.Int1 == 42);
                q.SortBy(i => i.Int1);
            });
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_WithTake_GeneratesCorrectQuery();

        protected SqlQuery On_GenerateQuery_WithTake_GeneratesCorrectQuery()
        {
            var queryCommand = GetQueryCommand<MyClass>(q => q.Take(11));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_WithTakeAndSorting_GeneratesCorrectQuery();

        protected SqlQuery On_GenerateQuery_WithTakeAndSorting_GeneratesCorrectQuery()
        {
            var queryCommand = GetQueryCommand<MyClass>(q =>
            {
                q.SortBy(i => i.Int1);
                q.Take(11);
            });
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_WithTakeAndWhereAndSorting_GeneratesCorrectQuery();

        protected SqlQuery On_GenerateQuery_WithTakeAndWhereAndSorting_GeneratesCorrectQuery()
        {
            var queryCommand = GetQueryCommand<MyClass>(q =>
            {
                q.Where(i => i.Int1 == 42);
                q.SortBy(i => i.Int1);
                q.Take(11);
            });
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_WithPagingAndWhereAndSorting_GeneratesCorrectQuery();

        protected SqlQuery On_GenerateQuery_WithPagingAndWhereAndSorting_GeneratesCorrectQuery()
        {
            var queryCommand = GetQueryCommand<MyClass>(q =>
            {
                q.Where(i => i.Int1 == 42);
                q.SortBy(i => i.Int1);
                q.Page(0, 10);
            });
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_WithExplicitSortingOnTwoDifferentMemberTypesAndSorting_GeneratesCorrectQuery();

        protected SqlQuery On_GenerateQuery_WithExplicitSortingOnTwoDifferentMemberTypesAndSorting_GeneratesCorrectQuery()
        {
            var queryCommand = GetQueryCommand<MyClass>(q => q.SortBy(i => i.Int1.Asc(), i => i.String1.Desc()));
            var generator = GetQueryGenerator();

            return generator.GenerateQuery(queryCommand);
        }

        public abstract void GenerateQuery_WithSortingOnTwoDifferentMemberOfSameType_GeneratesCorrectQuery();

        protected SqlQuery On_GenerateQuery_WithSortingOnTwoDifferentMemberOfSameType_GeneratesCorrectQuery()
        {
            var queryCommand = GetQueryCommand<MyClass>(q => q.SortBy(i => i.Int1, i => i.Int2));
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
            public int Int1 { get; set; }

            public int Int2 { get; set; }

            public string String1 { get; set; }
        }
    }
}