using NUnit.Framework;
using SisoDb.Lambdas.Processors;

namespace SisoDb.Tests.UnitTests.Lambdas.ParsedSelectorSqlProcessorTests
{
    [TestFixture]
    public class ParsedSelectorSqlProcessorMemberComparisionTests : ParsedSelectorSqlProcessorTestBase
    {
        [Test]
        public void Process_WhenMemberOfSameType_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 == i.Int2);

            var processor = new ParsedSelectorSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[Int1] = si.[Int2]";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenMemberOfSameType_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 == i.Int2);

            var processor = new ParsedSelectorSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            Assert.AreEqual(0, query.Parameters.Count);
        }
    }
}