using System;
using NUnit.Framework;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Converters.Sql;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.Querying.Sql;
using SisoDb.Tests.UnitTests.TestFactories;

namespace SisoDb.Tests.UnitTests.Querying.Sql
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
                "select s.Json from [dbo].[MyClassStructure] as s "
                + "inner join [dbo].[MyClassIndexes] as si on si.StructureId = s.StructureId where si.[Int1] = 42;",
                sqlQuery.Sql);
        }

        //[Test]
        //public void Generate_WithSorting_GeneratesCorrectSql()
        //{
        //    var queryCommand = GetQueryCommand(structureName: "MyClass", hasWhere: false, hasSortings: true);
        //    var generator = GetQueryGenerator(fakeWhere: string.Empty, fakeSorting: "si.[Int1] Asc");

        //    var sqlQuery = generator.GenerateQuery(queryCommand);

        //    const string expectedSql = "select s.Json from [dbo].[MyClassStructure] as s "
        //                               + "inner join [dbo].[MyClassIndexes] as si on si.StructureId = s.StructureId order by si.[Int1] Asc;";
        //    Assert.AreEqual(expectedSql, sqlQuery.Sql);
        //}

        //[Test]
        //public void Generate_WithWhereAndSorting_GeneratesCorrectSql()
        //{
        //    var queryCommand = GetQueryCommand(structureName: "MyClass", hasWhere: true, hasSortings: true);
        //    var generator = GetQueryGenerator(fakeWhere: "si.[Int1] = 42", fakeSorting: "si.[Int1] Desc");

        //    var sqlQuery = generator.GenerateQuery(queryCommand);

        //    const string expectedSql = "select s.Json from [dbo].[MyClassStructure] as s "
        //                               + "inner join [dbo].[MyClassIndexes] as si on si.StructureId = s.StructureId where si.[Int1] = 42 order by si.[Int1] Desc;";
        //    Assert.AreEqual(expectedSql, sqlQuery.Sql);
        //}

        //[Test]
        //public void Generate_WhenTakeWithOutWhereAndSorting_GeneratesCorrectSql()
        //{
        //    var queryCommand = GetQueryCommand(structureName: "MyClass", hasWhere: false, hasSortings: false, takeNumOfStructures: 11);
        //    var generator = GetQueryGenerator(fakeWhere: null, fakeSorting: null);

        //    var sqlQuery = generator.GenerateQuery(queryCommand);

        //    const string expectedSql = "select top(11) s.Json from [dbo].[MyClassStructure] as s "
        //                               + "inner join [dbo].[MyClassIndexes] as si on si.StructureId = s.StructureId;";
        //    Assert.AreEqual(expectedSql, sqlQuery.Sql);
        //}

        //[Test]
        //public void Generate_WithTakeAndSorting_GeneratesCorrectSql()
        //{
        //    var queryCommand = GetQueryCommand(structureName: "MyClass", hasWhere: false, hasSortings: true, takeNumOfStructures: 11);
        //    var generator = GetQueryGenerator(fakeWhere: null, fakeSorting: "si.[Int1] Desc");

        //    var sqlQuery = generator.GenerateQuery(queryCommand);

        //    const string expectedSql = "select top(11) s.Json from [dbo].[MyClassStructure] as s "
        //                               + "inner join [dbo].[MyClassIndexes] as si on si.StructureId = s.StructureId order by si.[Int1] Desc;";
        //    Assert.AreEqual(expectedSql, sqlQuery.Sql);
        //}

        //[Test]
        //public void Generate_WithTakeAndWhereAndSorting_GeneratesCorrectSql()
        //{
        //    var queryCommand = GetQueryCommand(structureName: "MyClass", hasWhere: true, hasSortings: true, takeNumOfStructures: 11);
        //    var generator = GetQueryGenerator(fakeWhere: "si.[Int1] = 42", fakeSorting: "si.[Int1] Desc");

        //    var sqlQuery = generator.GenerateQuery(queryCommand);

        //    const string expectedSql = "select top(11) s.Json from [dbo].[MyClassStructure] as s "
        //                               + "inner join [dbo].[MyClassIndexes] as si on si.StructureId = s.StructureId where si.[Int1] = 42 order by si.[Int1] Desc;";
        //    Assert.AreEqual(expectedSql, sqlQuery.Sql);
        //}

        //[Test]
        //public void Generate_WithPagingAndWhereAndSorting_GeneratesCorrectSql()
        //{
        //    var queryCommand = GetQueryCommand(structureName: "MyClass", hasWhere: true, hasSortings: true, paging: new Paging(0, 10));
        //    var generator = GetQueryGenerator(fakeWhere: "si.[Int1] = 42", fakeSorting: "si.[Int1] Desc");

        //    var sqlQuery = generator.GenerateQuery(queryCommand);

        //    const string expectedSql =
        //        "with pagedRs as (select s.Json,row_number() over ( order by si.[Int1] Desc) RowNum "
        //        + "from [dbo].[MyClassStructure] as s inner join [dbo].[MyClassIndexes] as si on si.StructureId = s.StructureId "
        //        + "where si.[Int1] = 42)select Json from pagedRs where RowNum between @pagingFrom and @pagingTo;";
                
        //    Assert.AreEqual(expectedSql, sqlQuery.Sql);
        //    Assert.AreEqual("@pagingFrom", sqlQuery.Parameters[0].Name);
        //    Assert.AreEqual(1, sqlQuery.Parameters[0].Value);
        //    Assert.AreEqual("@pagingTo", sqlQuery.Parameters[1].Name);
        //    Assert.AreEqual(10, sqlQuery.Parameters[1].Value);
        //}

        private static IQueryCommand GetQueryCommand<T>(Action<IQueryCommandBuilder<T>> commandInitializer) where T : class
        {
            var schema = StructureSchemaTestFactory.Stub<T>();//new StructureSchemas(new StructureTypeFactory(), new AutoSchemaBuilder()).GetSchema<T>();
            var builder = new QueryCommandBuilder<T>(schema, new WhereParser(), new SortingParser(), new IncludeParser());
            
            commandInitializer(builder);

            return builder.Command;
        }

        private static SqlQueryGenerator GetQueryGenerator()
        {
            return new SqlQueryGenerator(
                new LambdaToSqlWhereConverter(), 
                new LambdaToSqlSortingConverter(), 
                new LambdaToSqlIncludeConverter());
        }

        private class MyClass
        {
            public int Int1 { get; set; }
        }

        private class ChildClass
        {

        }
    }
}