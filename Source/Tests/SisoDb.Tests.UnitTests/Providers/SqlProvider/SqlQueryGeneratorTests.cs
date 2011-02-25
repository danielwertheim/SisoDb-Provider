using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using SisoDb.Lambdas;
using SisoDb.Lambdas.Parsers;
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
        public void Generate_WhenOnlyWhereSelector_GeneratesSelectJoinAndWhereButNoSorting()
        {
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.Setup(x => x.Name).Returns("MyClass");
            var queryCommand = GetQueryCommandStub(hasSelector: true, hasSortings: false);
            var generator = GetIsolatedQueryGenerator(fakeSelector: "si.[Int1] = 42", fakeSorting: string.Empty);

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
            var queryCommand = GetQueryCommandStub(hasSelector: false, hasSortings: true);
            var generator = GetIsolatedQueryGenerator(fakeSelector: string.Empty, fakeSorting: "si.[Int1] Asc");

            var sqlQuery = generator.Generate(queryCommand, schemaFake.Object);

            const string expectedSql = "select s.Json from [dbo].[MyClassStructure] as s "
                                       + "inner join [dbo].[MyClassIndexes] as si on si.StructureId = s.Id order by si.[Int1] Asc;";
            Assert.AreEqual(expectedSql, sqlQuery.Value);
        }

        [Test]
        public void Generate_WhenWhereSelectorAndSorting_GeneratesSelectJoinWhereAndSorting()
        {
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.Setup(x => x.Name).Returns("MyClass");
            var queryCommand = GetQueryCommandStub(hasSelector: true, hasSortings: true);
            var generator = GetIsolatedQueryGenerator(fakeSelector: "si.[Int1] = 42", fakeSorting: "si.[Int1] Desc");

            var sqlQuery = generator.Generate(queryCommand, schemaFake.Object);

            const string expectedSql = "select s.Json from [dbo].[MyClassStructure] as s "
                                       + "inner join [dbo].[MyClassIndexes] as si on si.StructureId = s.Id where si.[Int1] = 42 order by si.[Int1] Desc;";
            Assert.AreEqual(expectedSql, sqlQuery.Value);
        }

        private static IQueryCommand GetQueryCommandStub(bool hasSelector, bool hasSortings)
        {
            var stub = new Mock<IQueryCommand>();
            stub.Setup(s => s.HasSelector).Returns(hasSelector);
            stub.Setup(s => s.HasSortings).Returns(hasSortings);

            return stub.Object;
        }

        private static SqlQueryGenerator GetIsolatedQueryGenerator(string fakeSelector, string fakeSorting) 
        {
            var sqlSelectorFake = new Mock<ISqlSelector>();
            sqlSelectorFake.Setup(x => x.Sql).Returns(fakeSelector);
            sqlSelectorFake.Setup(x => x.Parameters).Returns(new List<IQueryParameter>());

            var sqlSortingFake = new Mock<ISqlSorting>();
            sqlSortingFake.Setup(x => x.Sql).Returns(fakeSorting);

            var sqlIncludeFake = new Mock<ISqlInclude>();
            sqlIncludeFake.Setup(x => x.Sql).Returns("");

            var parsedSelectorProcessorFake = new Mock<IParsedLambdaProcessor<ISqlSelector>>();
            parsedSelectorProcessorFake.Setup(x => x.Process(It.IsAny<IParsedLambda>())).Returns(sqlSelectorFake.Object);

            var parsedSortingProcessorFake = new Mock<IParsedLambdaProcessor<ISqlSorting>>();
            parsedSortingProcessorFake.Setup(x => x.Process(It.IsAny<IParsedLambda>())).Returns(sqlSortingFake.Object);

            var parsedIncludeProcessorFake = new Mock<IParsedLambdaProcessor<IList<ISqlInclude>>>();
            parsedIncludeProcessorFake.Setup(x => x.Process(It.IsAny<IParsedLambda>())).Returns(new []{sqlIncludeFake.Object});

            return new SqlQueryGenerator(
                                         parsedSelectorProcessorFake.Object, 
                                         parsedSortingProcessorFake.Object, 
                                         parsedIncludeProcessorFake.Object);
        }

        public class MyClass
        {
            public int Int1 { get; set; }
        }

        public class ChildClass
        {
            
        }
    }
}