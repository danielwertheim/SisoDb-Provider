using NUnit.Framework;
using SisoDb.Lambdas.Processors;
using SisoDb.Querying;

namespace SisoDb.Tests.UnitTests.Lambdas.ParsedLambdaSqlProcessorTests
{
    [TestFixture]
    public class ParsedLambdaSqlProcessorCombinedTests : ParsedLambdaSqlProcessorTestBase
    {
        [Test]
        public void Process_WhenCombinedMembers_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i =>
                (i.Int1 == 42 && i.String1 == "A") ||
                (i.Int1 == 11 && i.String1 == "AA") ||
                i.Int1 == 99);

            var processor = new ParsedLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "(((si.[Int1] = @p0 and si.[String1] = @p1) or (si.[Int1] = @p2 and si.[String1] = @p3)) or si.[Int1] = @p4)";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenCombinedMembers_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i =>
                (i.Int1 == 42 && i.String1 == "A") ||
                (i.Int1 == 11 && i.String1 == "AA") ||
                i.Int1 == 99);

            var processor = new ParsedLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[]
            {
                new QueryParameter("@p0", 42),
                new QueryParameter("@p1", "A"),
                new QueryParameter("@p2", 11),
                new QueryParameter("@p3", "AA"),
                new QueryParameter("@p4", 99)
            };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }
    }
}