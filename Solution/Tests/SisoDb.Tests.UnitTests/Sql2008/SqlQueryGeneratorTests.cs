using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SisoDb.Dac;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas;
using SisoDb.Querying.Lambdas.Processors;
using SisoDb.Querying.Sql;
using SisoDb.Sql2008;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.UnitTests.Sql2008
{
    [TestFixture]
    public class SqlQueryGeneratorTests : UnitTestBase
    {
        [Test]
        public void Generate_WithWhere_GeneratesCorrectSql()
        {
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.Setup(x => x.Name).Returns("MyClass");
            var queryCommand = GetQueryCommandStub(hasWhere: true, hasSortings: false);
            var generator = GetIsolatedQueryGenerator(fakeWhere: "si.[Int1] = 42", fakeSorting: string.Empty);

            var sqlQuery = generator.Generate(queryCommand, schemaFake.Object);

            const string expectedSql = "select s.Json from [dbo].[MyClassStructure] as s "
                                       + "inner join [dbo].[MyClassIndexes] as si on si.SisoId = s.SisoId where si.[Int1] = 42;";
            Assert.AreEqual(expectedSql, sqlQuery.Sql);
        }

        [Test]
        public void Generate_WithSorting_GeneratesCorrectSql()
        {
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.Setup(x => x.Name).Returns("MyClass");
            var queryCommand = GetQueryCommandStub(hasWhere: false, hasSortings: true);
            var generator = GetIsolatedQueryGenerator(fakeWhere: string.Empty, fakeSorting: "si.[Int1] Asc");

            var sqlQuery = generator.Generate(queryCommand, schemaFake.Object);

            const string expectedSql = "select s.Json from [dbo].[MyClassStructure] as s "
                                       + "inner join [dbo].[MyClassIndexes] as si on si.SisoId = s.SisoId order by si.[Int1] Asc;";
            Assert.AreEqual(expectedSql, sqlQuery.Sql);
        }

        [Test]
        public void Generate_WithWhereAndSorting_GeneratesCorrectSql()
        {
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.Setup(x => x.Name).Returns("MyClass");
            var queryCommand = GetQueryCommandStub(hasWhere: true, hasSortings: true);
            var generator = GetIsolatedQueryGenerator(fakeWhere: "si.[Int1] = 42", fakeSorting: "si.[Int1] Desc");

            var sqlQuery = generator.Generate(queryCommand, schemaFake.Object);

            const string expectedSql = "select s.Json from [dbo].[MyClassStructure] as s "
                                       + "inner join [dbo].[MyClassIndexes] as si on si.SisoId = s.SisoId where si.[Int1] = 42 order by si.[Int1] Desc;";
            Assert.AreEqual(expectedSql, sqlQuery.Sql);
        }

        [Test]
        public void Generate_WhenTakeWithOutWhereAndSorting_GeneratesCorrectSql()
        {
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.Setup(x => x.Name).Returns("MyClass");
            var queryCommand = GetQueryCommandStub(hasWhere: false, hasSortings: false, takeNumOfStructures: 11);
            var generator = GetIsolatedQueryGenerator(fakeWhere: null, fakeSorting: null);

            var sqlQuery = generator.Generate(queryCommand, schemaFake.Object);

            const string expectedSql = "select top(11) s.Json from [dbo].[MyClassStructure] as s "
                                       + "inner join [dbo].[MyClassIndexes] as si on si.SisoId = s.SisoId;";
            Assert.AreEqual(expectedSql, sqlQuery.Sql);
        }

        [Test]
        public void Generate_WithTakeAndSorting_GeneratesCorrectSql()
        {
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.Setup(x => x.Name).Returns("MyClass");
            var queryCommand = GetQueryCommandStub(hasWhere: false, hasSortings: true, takeNumOfStructures: 11);
            var generator = GetIsolatedQueryGenerator(fakeWhere: null, fakeSorting: "si.[Int1] Desc");

            var sqlQuery = generator.Generate(queryCommand, schemaFake.Object);

            const string expectedSql = "select top(11) s.Json from [dbo].[MyClassStructure] as s "
                                       + "inner join [dbo].[MyClassIndexes] as si on si.SisoId = s.SisoId order by si.[Int1] Desc;";
            Assert.AreEqual(expectedSql, sqlQuery.Sql);
        }

        [Test]
        public void Generate_WithTakeAndWhereAndSorting_GeneratesCorrectSql()
        {
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.Setup(x => x.Name).Returns("MyClass");
            var queryCommand = GetQueryCommandStub(hasWhere: true, hasSortings: true, takeNumOfStructures: 11);
            var generator = GetIsolatedQueryGenerator(fakeWhere: "si.[Int1] = 42", fakeSorting: "si.[Int1] Desc");

            var sqlQuery = generator.Generate(queryCommand, schemaFake.Object);

            const string expectedSql = "select top(11) s.Json from [dbo].[MyClassStructure] as s "
                                       + "inner join [dbo].[MyClassIndexes] as si on si.SisoId = s.SisoId where si.[Int1] = 42 order by si.[Int1] Desc;";
            Assert.AreEqual(expectedSql, sqlQuery.Sql);
        }

        [Test]
        public void Generate_WithPagingAndWhereAndSorting_GeneratesCorrectSql()
        {
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.Setup(x => x.Name).Returns("MyClass");
            var queryCommand = GetQueryCommandStub(hasWhere: true, hasSortings: true, paging: new Paging(0, 10));
            var generator = GetIsolatedQueryGenerator(fakeWhere: "si.[Int1] = 42", fakeSorting: "si.[Int1] Desc");

            var sqlQuery = generator.Generate(queryCommand, schemaFake.Object);

            const string expectedSql =
                "with pagedRs as (select s.Json,row_number() over ( order by si.[Int1] Desc) RowNum "
                + "from [dbo].[MyClassStructure] as s inner join [dbo].[MyClassIndexes] as si on si.SisoId = s.SisoId "
                + "where si.[Int1] = 42)select Json from pagedRs where RowNum between @pagingFrom and @pagingTo;";
                
            Assert.AreEqual(expectedSql, sqlQuery.Sql);
            Assert.AreEqual("@pagingFrom", sqlQuery.Parameters[0].Name);
            Assert.AreEqual(1, sqlQuery.Parameters[0].Value);
            Assert.AreEqual("@pagingTo", sqlQuery.Parameters[1].Name);
            Assert.AreEqual(10, sqlQuery.Parameters[1].Value);
        }

        private static IQueryCommand GetQueryCommandStub(bool hasWhere, bool hasSortings, Paging paging = null, int? takeNumOfStructures = null)
        {
            var stub = new Mock<IQueryCommand>();
            stub.Setup(s => s.HasWhere).Returns(hasWhere);
            stub.Setup(s => s.HasSortings).Returns(hasSortings);
            stub.Setup(s => s.HasPaging).Returns(paging != null);

            if (paging != null)
                stub.Setup(s => s.Paging).Returns(paging);

            if (takeNumOfStructures.HasValue)
            {
                stub.Setup(s => s.HasTakeNumOfStructures).Returns(true);
                stub.Setup(s => s.TakeNumOfStructures).Returns(takeNumOfStructures.Value);
            }

            return stub.Object;
        }

        private static SqlQueryGenerator GetIsolatedQueryGenerator(string fakeWhere, string fakeSorting)
        {
            var sqlWhereFake = new Mock<ISqlWhere>();
            sqlWhereFake.Setup(x => x.Sql).Returns(fakeWhere);
            sqlWhereFake.Setup(x => x.Parameters).Returns(new List<IDacParameter>());

            var sqlSortingFake = new Mock<ISqlSorting>();
            sqlSortingFake.Setup(x => x.Sql).Returns(fakeSorting);

            var sqlIncludeFake = new Mock<ISqlInclude>();
            sqlIncludeFake.Setup(x => x.Sql).Returns("");

            var whereProcessorFake = new Mock<IParsedLambdaProcessor<ISqlWhere>>();
            whereProcessorFake.Setup(x => x.Process(It.IsAny<IParsedLambda>())).Returns(sqlWhereFake.Object);

            var sortingsProcessorFake = new Mock<IParsedLambdaProcessor<ISqlSorting>>();
            sortingsProcessorFake.Setup(x => x.Process(It.IsAny<IParsedLambda>())).Returns(sqlSortingFake.Object);

            var includesProcessorFake = new Mock<IParsedLambdaProcessor<IList<ISqlInclude>>>();
            includesProcessorFake.Setup(x => x.Process(It.IsAny<IParsedLambda>())).Returns(new[] { sqlIncludeFake.Object });

            return new SqlQueryGenerator(
                                         whereProcessorFake.Object,
                                         sortingsProcessorFake.Object,
                                         includesProcessorFake.Object);
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