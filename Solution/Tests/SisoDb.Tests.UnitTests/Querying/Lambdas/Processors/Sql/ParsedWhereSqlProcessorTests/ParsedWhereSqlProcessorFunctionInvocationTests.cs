using System;
using NUnit.Framework;
using SisoDb.Dac;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Processors.Sql;

namespace SisoDb.Tests.UnitTests.Querying.Lambdas.Processors.Sql.ParsedWhereSqlProcessorTests
{
    [TestFixture]
    public class ParsedWhereSqlProcessorFunctionInvocationTests : ParsedWhereSqlProcessorTestBase
    {
        [Test]
        public void Process_WhenStaticFunction_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.DateTime1 == DateTime.Parse("2010-11-12"));

            var processor = new ParsedWhereSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[DateTime1] = @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenStaticFunction_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.DateTime1 == DateTime.Parse("2010-11-12"));

            var processor = new ParsedWhereSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", new DateTime(2010, 11, 12)) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenStaticFunctionWithMultiArgs_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.DateTime1 == DateTime.Parse("2010-11-12", SisoEnvironment.Formatting.DateTimeFormatProvider));

            var processor = new ParsedWhereSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[DateTime1] = @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenStaticFunctionWithMultiArgs_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.DateTime1 == DateTime.Parse("2010-11-12", SisoEnvironment.Formatting.DateTimeFormatProvider));

            var processor = new ParsedWhereSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", new DateTime(2010, 11, 12)) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenInstanceFunction_GeneratesCorrectSqlQuery()
        {
            var dateTime = new DateTime(2010, 11, 12);
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == dateTime.ToString(SisoEnvironment.Formatting.DateTimeFormatProvider));

            var processor = new ParsedWhereSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[String1] = @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenInstanceFunction_ExtractsCorrectParameters()
        {
            var dateTime = new DateTime(2010, 11, 12);
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == dateTime.ToString(SisoEnvironment.Formatting.DateTimeFormatProvider));

            var processor = new ParsedWhereSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", "2010-11-12 00:00:00") };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }
    }
}