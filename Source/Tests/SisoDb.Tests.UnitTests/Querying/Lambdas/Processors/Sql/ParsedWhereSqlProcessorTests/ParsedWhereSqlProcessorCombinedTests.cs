using NUnit.Framework;
using SisoDb.Dac;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Processors.Sql;

namespace SisoDb.Tests.UnitTests.Querying.Lambdas.Processors.Sql.ParsedWhereSqlProcessorTests
{
    [TestFixture]
    public class ParsedWhereSqlProcessorCombinedTests : ParsedWhereSqlProcessorTestBase
    {
        [Test]
        public void Process_WhenCombinedMembers_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i =>
                (i.Int1 == 42 && i.String1 == "A") ||
                (i.Int1 == 11 && i.String1 == "AA") ||
                i.Int1 == 99);

            var processor = new ParsedWhereSqlProcessor(new MemberNameGeneratorFake());
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

            var processor = new ParsedWhereSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[]
            {
                new DacParameter("@p0", 42),
                new DacParameter("@p1", "A"),
                new DacParameter("@p2", 11),
                new DacParameter("@p3", "AA"),
                new DacParameter("@p4", 99)
            };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }
    }
}