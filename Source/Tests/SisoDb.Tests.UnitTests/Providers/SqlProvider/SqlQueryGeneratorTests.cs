using NUnit.Framework;
using SisoDb.Lambdas;
using SisoDb.Lambdas.Processors;
using SisoDb.Providers.SqlProvider;
using SisoDb.Querying;
using SisoDb.Structures.Schemas;
using TypeMock.ArrangeActAssert;

namespace SisoDb.Tests.UnitTests.Providers.SqlProvider
{
    [TestFixture]
    public class SqlQueryGeneratorTests
    {
        [Test, Isolated]
        public void Generate_WithMockedDependencies_GeneratesCorrectSelectAndJoinSyntax()
        {
            var parsedLambdaFake = Isolate.Fake.Instance<IParsedLambda>();
            var lamdaParserFake = Isolate.Fake.Instance<ILambdaParser>();
            Isolate.WhenCalled(() => lamdaParserFake.Parse<MyClass>(null)).WillReturn(parsedLambdaFake);
            var sqlQueryFake = Isolate.Fake.Instance<ISqlQuery>();
            Isolate.WhenCalled(() => sqlQueryFake.Sql).WillReturn("1 = 1");
            var parsedLambdaProcessorFake = Isolate.Fake.Instance<IParsedLambdaProcessor<ISqlQuery>>();
            Isolate.WhenCalled(() => parsedLambdaProcessorFake.Process(null)).WillReturn(sqlQueryFake);
            var schemaFake = Isolate.Fake.Instance<IStructureSchema>();
            Isolate.WhenCalled(() => schemaFake.GetIndexesTableName()).WillReturn("MyClassIndexes");
            Isolate.WhenCalled(() => schemaFake.GetStructureTableName()).WillReturn("MyClassStructure");

            var generator = new SqlQueryGenerator(lamdaParserFake, parsedLambdaProcessorFake);
            var sqlQuery = generator.Generate<MyClass>(m => m.Int1 == 42, schemaFake);

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