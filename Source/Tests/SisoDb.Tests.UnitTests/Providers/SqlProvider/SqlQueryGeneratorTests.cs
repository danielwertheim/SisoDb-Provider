using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SisoDb.Lambdas;
using SisoDb.Lambdas.Processors;
using SisoDb.Providers.SqlProvider;
using SisoDb.Querying;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.UnitTests.Providers.SqlProvider
{
    [TestFixture]
    public class SqlQueryGeneratorTests : UnitTestBase
    {
        [Test]
        public void Generate_WhenOnlyWhere_GeneratesSelectJoinAndWhereButNoSorting()
        {
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.Setup(x => x.Name).Returns("MyClass");
            var queryCommand = GetQueryCommandStub(hasWhere: true, hasSortings: false);
            var generator = GetIsolatedQueryGenerator(fakeWhere: "si.[Int1] = 42", fakeSorting: string.Empty);

            var sqlQuery = generator.Generate(queryCommand, schemaFake.Object);

            const string expectedSql = "select s.Json from [dbo].[MyClassStructure] as s "
                                       + "inner join [dbo].[MyClassIndexes] as si on si.StructureId = s.Id where si.[Int1] = 42;";
            Assert.AreEqual(expectedSql, sqlQuery.Value);
        }

        [Test]
        public void Generate_WhenOnlySorting_GeneratesSelectJoinAndSortingButNoWhere()
        {
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.Setup(x => x.Name).Returns("MyClass");
            var queryCommand = GetQueryCommandStub(hasWhere: false, hasSortings: true);
            var generator = GetIsolatedQueryGenerator(fakeWhere: string.Empty, fakeSorting: "si.[Int1] Asc");

            var sqlQuery = generator.Generate(queryCommand, schemaFake.Object);

            const string expectedSql = "select s.Json from [dbo].[MyClassStructure] as s "
                                       + "inner join [dbo].[MyClassIndexes] as si on si.StructureId = s.Id order by si.[Int1] Asc;";
            Assert.AreEqual(expectedSql, sqlQuery.Value);
        }

        [Test]
        public void Generate_WhenWhereAndSorting_GeneratesSelectJoinWhereAndSorting()
        {
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.Setup(x => x.Name).Returns("MyClass");
            var queryCommand = GetQueryCommandStub(hasWhere: true, hasSortings: true);
            var generator = GetIsolatedQueryGenerator(fakeWhere: "si.[Int1] = 42", fakeSorting: "si.[Int1] Desc");

            var sqlQuery = generator.Generate(queryCommand, schemaFake.Object);

            const string expectedSql = "select s.Json from [dbo].[MyClassStructure] as s "
                                       + "inner join [dbo].[MyClassIndexes] as si on si.StructureId = s.Id where si.[Int1] = 42 order by si.[Int1] Desc;";
            Assert.AreEqual(expectedSql, sqlQuery.Value);
        }

        [Test]
        public void Generate_WhenTakeWithOutWhereAndSorting_TopWithOutOrderByIsRepresented()
        {
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.Setup(x => x.Name).Returns("MyClass");
            var queryCommand = GetQueryCommandStub(hasWhere: false, hasSortings: false, takeNumOfStructures: 11);
            var generator = GetIsolatedQueryGenerator(fakeWhere: null, fakeSorting: null);

            var sqlQuery = generator.Generate(queryCommand, schemaFake.Object);

            const string expectedSql = "select top(11) s.Json from [dbo].[MyClassStructure] as s "
                                       + "inner join [dbo].[MyClassIndexes] as si on si.StructureId = s.Id;";
            Assert.AreEqual(expectedSql, sqlQuery.Value);
        }

        [Test]
        public void Generate_WhenTakeAndSorting_TopWithOrderByIsRepresented()
        {
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.Setup(x => x.Name).Returns("MyClass");
            var queryCommand = GetQueryCommandStub(hasWhere: false, hasSortings: true, takeNumOfStructures: 11);
            var generator = GetIsolatedQueryGenerator(fakeWhere: null, fakeSorting: "si.[Int1] Desc");

            var sqlQuery = generator.Generate(queryCommand, schemaFake.Object);

            const string expectedSql = "select top(11) s.Json from [dbo].[MyClassStructure] as s "
                                       + "inner join [dbo].[MyClassIndexes] as si on si.StructureId = s.Id order by si.[Int1] Desc;";
            Assert.AreEqual(expectedSql, sqlQuery.Value);
        }

        [Test]
        public void Generate_WhenTakeAndWhereAndSorting_TopWithWhereAndOrderByIsRepresented()
        {
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.Setup(x => x.Name).Returns("MyClass");
            var queryCommand = GetQueryCommandStub(hasWhere: true, hasSortings: true, takeNumOfStructures: 11);
            var generator = GetIsolatedQueryGenerator(fakeWhere: "si.[Int1] = 42", fakeSorting: "si.[Int1] Desc");

            var sqlQuery = generator.Generate(queryCommand, schemaFake.Object);

            const string expectedSql = "select top(11) s.Json from [dbo].[MyClassStructure] as s "
                                       + "inner join [dbo].[MyClassIndexes] as si on si.StructureId = s.Id where si.[Int1] = 42 order by si.[Int1] Desc;";
            Assert.AreEqual(expectedSql, sqlQuery.Value);
        }

        private static IQueryCommand GetQueryCommandStub(bool hasWhere, bool hasSortings, int? takeNumOfStructures = null)
        {
            var stub = new Mock<IQueryCommand>();
            stub.Setup(s => s.HasWhere).Returns(hasWhere);
            stub.Setup(s => s.HasSortings).Returns(hasSortings);

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
            sqlWhereFake.Setup(x => x.Parameters).Returns(new List<IQueryParameter>());

            var sqlSortingFake = new Mock<ISqlSorting>();
            sqlSortingFake.Setup(x => x.Sql).Returns(fakeSorting);

            var sqlIncludeFake = new Mock<ISqlInclude>();
            sqlIncludeFake.Setup(x => x.Sql).Returns("");
            
            var whereProcessorFake = new Mock<IParsedLambdaProcessor<ISqlWhere>>();
            whereProcessorFake.Setup(x => x.Process(It.IsAny<IParsedLambda>())).Returns(sqlWhereFake.Object);

            var sortingsProcessorFake = new Mock<IParsedLambdaProcessor<ISqlSorting>>();
            sortingsProcessorFake.Setup(x => x.Process(It.IsAny<IParsedLambda>())).Returns(sqlSortingFake.Object);

            var includesProcessorFake = new Mock<IParsedLambdaProcessor<IList<ISqlInclude>>>();
            includesProcessorFake.Setup(x => x.Process(It.IsAny<IParsedLambda>())).Returns(new []{sqlIncludeFake.Object});

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