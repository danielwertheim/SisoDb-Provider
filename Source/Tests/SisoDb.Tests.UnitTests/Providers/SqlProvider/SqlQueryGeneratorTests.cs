using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        public void Generate_WhenOnlyWhereSelector_GeneratesSelectJoinAndWhereButNoSorting()
        {
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.Setup(x => x.Name).Returns("MyClass");
            var queryCommand = GetQueryCommandStub<MyClass>(hasSelector: true, hasSortings: false);
            var generator = GetIsolatedQueryGenerator(fakeSelector: "si.[Int1] = 42", fakeSorting: string.Empty);

            var sqlQuery = generator.Generate(queryCommand, schemaFake.Object);

            const string expectedSql = "select s.Json from [dbo].[MyClassStructure] as s "
                                       + "inner join [dbo].[MyClassIndexes] as si on si.StructureId = s.Id where si.[Int1] = 42;";
            Assert.AreEqual(expectedSql, sqlQuery.Sql);
        }

        [Test]
        public void Generate_WhenOnlySorting_GeneratesSelectJoinAndSortingButNoWhere()
        {
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.Setup(x => x.Name).Returns("MyClass");
            var queryCommand = GetQueryCommandStub<MyClass>(hasSelector: false, hasSortings: true);
            var generator = GetIsolatedQueryGenerator(fakeSelector: string.Empty, fakeSorting: "si.[Int1] Asc");

            var sqlQuery = generator.Generate(queryCommand, schemaFake.Object);

            const string expectedSql = "select s.Json from [dbo].[MyClassStructure] as s "
                                       + "inner join [dbo].[MyClassIndexes] as si on si.StructureId = s.Id order by si.[Int1] Asc;";
            Assert.AreEqual(expectedSql, sqlQuery.Sql);
        }

        [Test]
        public void Generate_WhenWhereSelectorAndSorting_GeneratesSelectJoinWhereAndSorting()
        {
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.Setup(x => x.Name).Returns("MyClass");
            var queryCommand = GetQueryCommandStub<MyClass>(hasSelector: true, hasSortings: true);
            var generator = GetIsolatedQueryGenerator(fakeSelector: "si.[Int1] = 42", fakeSorting: "si.[Int1] Desc");

            var sqlQuery = generator.Generate(queryCommand, schemaFake.Object);

            const string expectedSql = "select s.Json from [dbo].[MyClassStructure] as s "
                                       + "inner join [dbo].[MyClassIndexes] as si on si.StructureId = s.Id where si.[Int1] = 42 order by si.[Int1] Desc;";
            Assert.AreEqual(expectedSql, sqlQuery.Sql);
        }

        private static IQueryCommand<T> GetQueryCommandStub<T>(bool hasSelector, bool hasSortings) where T : class
        {
            var stub = new Mock<IQueryCommand<T>>();
            stub.Setup(s => s.HasSelector).Returns(hasSelector);
            stub.Setup(s => s.HasSortings).Returns(hasSortings);

            return stub.Object;
        }

        private static SqlQueryGenerator GetIsolatedQueryGenerator(string fakeSelector, string fakeSorting)
        {
            var parsedLambdaFake = new Mock<IParsedLambda>();
            
            var selectorParserFake = new Mock<ISelectorParser>();
            selectorParserFake.Setup(x => x.Parse(It.IsAny<Expression<Func<MyClass, bool>>>())).Returns(parsedLambdaFake.Object);

            var sortingParserFake = new Mock<ISortingParser>();
            sortingParserFake.Setup(x => x.Parse(It.IsAny<Expression<Func<MyClass, bool>>[]>())).Returns(parsedLambdaFake.Object);

            var sqlSelectorFake = new Mock<ISqlSelector>();
            sqlSelectorFake.Setup(x => x.Sql).Returns(fakeSelector);
            sqlSelectorFake.Setup(x => x.Parameters).Returns(new List<IQueryParameter>());

            var sqlSortingFake = new Mock<ISqlSorting>();
            sqlSortingFake.Setup(x => x.Sql).Returns(fakeSorting);

            var parsedSelectorProcessorFake = new Mock<IParsedLambdaProcessor<ISqlSelector>>();
            parsedSelectorProcessorFake.Setup(x => x.Process(It.IsAny<IParsedLambda>())).Returns(sqlSelectorFake.Object);

            var parsedSortingProcessorFake = new Mock<IParsedLambdaProcessor<ISqlSorting>>();
            parsedSortingProcessorFake.Setup(x => x.Process(It.IsAny<IParsedLambda>())).Returns(sqlSortingFake.Object);

            return new SqlQueryGenerator(selectorParserFake.Object, sortingParserFake.Object,
                                         parsedSelectorProcessorFake.Object, parsedSortingProcessorFake.Object);
        }

        public class MyClass
        {
            public int Int1 { get; set; }
        }
    }
}