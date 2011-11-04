using System;
using NUnit.Framework;
using SisoDb.Sql2008;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Converters.Sql;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.UnitTests.TestFactories;

namespace SisoDb.UnitTests.Querying.Sql
{
    [TestFixture]
    public class SqlQueryGeneratorTests : UnitTestBase
    {
        [Test]
        public void Generate_WithWhere_GeneratesCorrectSql()
        {
            var queryCommand = GetQueryCommand<MyClass>(q => q.Where(i => i.Int1 == 42));
            var generator = GetQueryGenerator();

            var sqlQuery = generator.GenerateQuery(queryCommand);

            Assert.AreEqual(
                "select s.Json from [MyClassStructure] as s " +
                "inner join [MyClassIndexes] as si on si.[StructureId] = s.[StructureId] " + 
                "where (si.[MemberPath]='Int1' and si.[IntegerValue] = @p0) " +
                "group by s.[StructureId], s.[Json] " + 
                "order by s.[StructureId];",
                sqlQuery.Sql);
        }

        [Test]
        public void Generate_WithSorting_GeneratesCorrectSql()
        {
            var queryCommand = GetQueryCommand<MyClass>(q => q.SortBy(i => i.Int1));
            var generator = GetQueryGenerator();

            var sqlQuery = generator.GenerateQuery(queryCommand);

            Assert.AreEqual(
                "select s.Json from [MyClassStructure] as s " +
                "inner join [MyClassIndexes] as si on si.[StructureId] = s.[StructureId] " + 
                "group by s.[StructureId], s.[Json] " +
                "order by min(si.[IntegerValue]) Asc;",
                sqlQuery.Sql);
        }

        [Test]
        public void Generate_WithWhereAndSorting_GeneratesCorrectSql()
        {
            var queryCommand = GetQueryCommand<MyClass>(q =>
            {
                q.Where(i => i.Int1 == 42);
                q.SortBy(i => i.Int1);
            });
            var generator = GetQueryGenerator();

            var sqlQuery = generator.GenerateQuery(queryCommand);

            Assert.AreEqual(
                "select s.Json from [MyClassStructure] as s " + 
                "inner join [MyClassIndexes] as si on si.[StructureId] = s.[StructureId] " + 
                "where (si.[MemberPath]='Int1' and si.[IntegerValue] = @p0) " +
                "group by s.[StructureId], s.[Json] " +
                "order by min(si.[IntegerValue]) Asc;",
                sqlQuery.Sql);
        }

        [Test]
        public void Generate_WhenTakeWithOutWhereAndSorting_GeneratesCorrectSql()
        {
            var queryCommand = GetQueryCommand<MyClass>(q => q.Take(11));
            var generator = GetQueryGenerator();

            var sqlQuery = generator.GenerateQuery(queryCommand);

            Assert.AreEqual(
                "select top(11) s.Json from [MyClassStructure] as s " +
                "inner join [MyClassIndexes] as si on si.[StructureId] = s.[StructureId] " +
                "group by s.[StructureId], s.[Json] order by s.[StructureId];",
                sqlQuery.Sql);
        }

        [Test]
        public void Generate_WithTakeAndSorting_GeneratesCorrectSql()
        {
            var queryCommand = GetQueryCommand<MyClass>(q =>
            {
                q.SortBy(i => i.Int1);
                q.Take(11);
            });
            var generator = GetQueryGenerator();

            var sqlQuery = generator.GenerateQuery(queryCommand);

            Assert.AreEqual(
                "select top(11) s.Json from [MyClassStructure] as s " +
                "inner join [MyClassIndexes] as si on si.[StructureId] = s.[StructureId] " +
                "group by s.[StructureId], s.[Json] " +
                "order by min(si.[IntegerValue]) Asc;",
                sqlQuery.Sql);
        }

        [Test]
        public void Generate_WithTakeAndWhereAndSorting_GeneratesCorrectSql()
        {
            var queryCommand = GetQueryCommand<MyClass>(q =>
            {
                q.Where(i => i.Int1 == 42);
                q.SortBy(i => i.Int1);
                q.Take(11);
            });
            var generator = GetQueryGenerator();

            var sqlQuery = generator.GenerateQuery(queryCommand);

            Assert.AreEqual(
                "select top(11) s.Json from [MyClassStructure] as s " +
                "inner join [MyClassIndexes] as si on si.[StructureId] = s.[StructureId] " +
                "where (si.[MemberPath]='Int1' and si.[IntegerValue] = @p0) " +
                "group by s.[StructureId], s.[Json] " +
                "order by min(si.[IntegerValue]) Asc;",
                sqlQuery.Sql);
        }

        [Test]
        public void Generate_WithPagingAndWhereAndSorting_GeneratesCorrectSql()
        {
            var queryCommand = GetQueryCommand<MyClass>(q =>
            {
                q.Where(i => i.Int1 == 42);
                q.SortBy(i => i.Int1);
                q.Page(0, 10);
            });
            var generator = GetQueryGenerator();

            var sqlQuery = generator.GenerateQuery(queryCommand);

            Assert.AreEqual(
                "with pagedRs as (select s.Json,row_number() over ( order by min(si.[IntegerValue]) Asc) RowNum " +
                "from [MyClassStructure] as s " +
                "inner join [MyClassIndexes] as si on si.[StructureId] = s.[StructureId] " +
                "where (si.[MemberPath]='Int1' and si.[IntegerValue] = @p0) " +
                "group by s.[StructureId], s.[Json])" +
                "select Json from pagedRs where RowNum between @pagingFrom and @pagingTo;",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual(42, sqlQuery.Parameters[0].Value);
            Assert.AreEqual("@pagingFrom", sqlQuery.Parameters[1].Name);
            Assert.AreEqual(1, sqlQuery.Parameters[1].Value);
            Assert.AreEqual("@pagingTo", sqlQuery.Parameters[2].Name);
            Assert.AreEqual(10, sqlQuery.Parameters[2].Value);
        }

        private static IQueryCommand GetQueryCommand<T>(Action<IQueryCommandBuilder<T>> commandInitializer) where T : class
        {
            var schema = StructureSchemaTestFactory.Stub<T>();//new StructureSchemas(new StructureTypeFactory(), new AutoSchemaBuilder()).GetSchema<T>();
            var builder = new QueryCommandBuilder<T>(schema, new WhereParser(), new SortingParser(), new IncludeParser());
            
            commandInitializer(builder);

            return builder.Command;
        }

        private static IDbQueryGenerator GetQueryGenerator()
        {
            return new Sql2008QueryGenerator(
                new LambdaToSqlWhereConverter(), 
                new LambdaToSqlSortingConverter(), 
                new LambdaToSqlIncludeConverter());
        }

        private class MyClass
        {
            public int Int1 { get; set; }
        }
    }
}