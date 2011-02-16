using System;
using NUnit.Framework;
using SisoDb.Lambdas.Processors;
using SisoDb.Querying;

namespace SisoDb.Tests.UnitTests.Lambdas.ParsedSelectorSqlProcessorTests
{
    [TestFixture]
    public class ParsedSelectorSqlProcessorFunctionInvocationTests : ParsedSelectorSqlProcessorTestBase
    {
        [Test]
        public void Process_WhenStaticFunction_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.DateTime1 == DateTime.Parse("2010-11-12"));

            var processor = new ParsedSelectorSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[DateTime1] = @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenStaticFunction_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.DateTime1 == DateTime.Parse("2010-11-12"));

            var processor = new ParsedSelectorSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new QueryParameter("@p0", new DateTime(2010, 11, 12)) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenStaticFunctionWithMultiArgs_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.DateTime1 == DateTime.Parse("2010-11-12", SisoDbEnvironment.Formatting.DateTimeFormatProvider));

            var processor = new ParsedSelectorSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[DateTime1] = @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenStaticFunctionWithMultiArgs_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.DateTime1 == DateTime.Parse("2010-11-12", SisoDbEnvironment.Formatting.DateTimeFormatProvider));

            var processor = new ParsedSelectorSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new QueryParameter("@p0", new DateTime(2010, 11, 12)) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenInstanceFunction_GeneratesCorrectSqlQuery()
        {
            var dateTime = new DateTime(2010, 11, 12);
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == dateTime.ToString(SisoDbEnvironment.Formatting.DateTimeFormatProvider));

            var processor = new ParsedSelectorSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[String1] = @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenInstanceFunction_ExtractsCorrectParameters()
        {
            var dateTime = new DateTime(2010, 11, 12);
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == dateTime.ToString(SisoDbEnvironment.Formatting.DateTimeFormatProvider));

            var processor = new ParsedSelectorSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new QueryParameter("@p0", "2010-11-12 00:00:00") };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }
    }
}