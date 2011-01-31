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
        public void Generate_WithMockedDependencies_GeneratesCorrectSelectAndJoinSyntax()
        {
            var parsedLambdaFake = new Mock<IParsedLambda>();
            var lamdaParserFake = new Mock<ILambdaParser>();
            lamdaParserFake.Setup(x => x.Parse(It.IsAny<Expression<Func<MyClass, bool>>>())).Returns(parsedLambdaFake.Object);
            var sqlQueryFake = new Mock<ISqlQuery>();
            sqlQueryFake.Setup(x => x.Sql).Returns("1 = 1");
            sqlQueryFake.Setup(x => x.Parameters).Returns(new List<IQueryParameter>());
            var parsedLambdaProcessorFake = new Mock<IParsedLambdaProcessor<ISqlQuery>>();
            parsedLambdaProcessorFake.Setup(x => x.Process(It.IsAny<IParsedLambda>())).Returns(sqlQueryFake.Object);
            var schemaFake = new Mock<IStructureSchema>();
            schemaFake.Setup(x => x.Name).Returns("MyClass");

            var generator = new SqlQueryGenerator(lamdaParserFake.Object, parsedLambdaProcessorFake.Object);
            var sqlQuery = generator.Generate<MyClass>(m => m.Int1 == 42, schemaFake.Object);

            var expectedSql = "select s.Json from [dbo].[MyClassStructure] as s " 
                + "inner join [dbo].[MyClassIndexes] as si on si.StructureId = s.Id where 1 = 1";
            Assert.AreEqual(expectedSql, sqlQuery.Sql);
        }

        private class MyClass
        {
            public int Int1 { get; set; }
        }
    }
}