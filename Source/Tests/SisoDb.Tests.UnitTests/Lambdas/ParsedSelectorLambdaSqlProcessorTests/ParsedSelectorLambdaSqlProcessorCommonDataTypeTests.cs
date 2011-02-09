using System;
using System.Linq;
using NUnit.Framework;
using SisoDb.Lambdas.Processors;
using SisoDb.Querying;

namespace SisoDb.Tests.UnitTests.Lambdas.ParsedSelectorLambdaSqlProcessorTests
{
    [TestFixture]
    public class ParsedSelectorLambdaSqlProcessorCommonDataTypeTests : ParsedSelectorLambdaSqlProcessorTestBase
    {
        [Test]
        public void Process_WhenInt_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 == 42);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[Int1] = @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenInt_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 == 42);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new QueryParameter("@p0", 42) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenNegativeInt_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 == -42);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[Int1] = @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenNegativeInt_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 == -42);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new QueryParameter("@p0", -42) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenIntNotEqualTo_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 != 42);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[Int1] <> @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenIntNotEqualTo_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 != 42);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new QueryParameter("@p0", 42) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenDecimal_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Decimal1 == 3.14M);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[Decimal1] = @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenDecimal_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Decimal1 == 3.14M);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new QueryParameter("@p0", 3.14M) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenFalseBool_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Bool1 == false);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[Bool1] = @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenFalseBool_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Bool1 == false);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new QueryParameter("@p0", false) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenTrueBool_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Bool1 == true);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[Bool1] = @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenTrueBool_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Bool1 == true);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new QueryParameter("@p0", true) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenNotEqualToTrueBool_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Bool1 != true);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[Bool1] <> @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenNotEqualToTrueBool_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Bool1 != true);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new QueryParameter("@p0", true) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenNotEqualToFalseBool_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Bool1 != false);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[Bool1] <> @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenNotEqualToFalseBool_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Bool1 != false);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new QueryParameter("@p0", false) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenGuidField_GeneratesCorrectSqlQuery()
        {
            var guid = Guid.Empty;
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Guid1 == guid);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[Guid1] = @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenGuidField_ExtractsCorrectParameters()
        {
            var guid = Guid.Empty;
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Guid1 == guid);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new QueryParameter("@p0", guid) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenGuidStaticField_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Guid1 == Guid.Empty);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[Guid1] = @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenGuidStaticField_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Guid1 == Guid.Empty);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new QueryParameter("@p0", Guid.Empty) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenDateTimeField_GeneratesCorrectSqlQuery()
        {
            var dateTime = new DateTime(2010, 2, 3, 12, 13, 14);
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.DateTime1 == dateTime);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[DateTime1] = @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenDateTimeField_ExtractsCorrectParameters()
        {
            var dateTime = new DateTime(2010, 2, 3, 12, 13, 14);
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.DateTime1 == dateTime);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new QueryParameter("@p0", dateTime) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenDateTimeStaticProperty_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.DateTime1 == DateTime.Now);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[DateTime1] = @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenDateTimeStaticProperty_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.DateTime1 == DateTime.Now);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameter = new QueryParameter("@p0", DateTime.Now);
            var actualParameter = query.Parameters.Single();
            var dateTimeNowDelta = DateTime.Now.Subtract((DateTime)actualParameter.Value);
            Assert.AreEqual(expectedParameter.Name, actualParameter.Name);
            Assert.AreEqual(0, dateTimeNowDelta.TotalSeconds, 1);
        }

        [Test]
        public void Process_WhenString_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == "Adam");

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[String1] = @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenString_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == "Adam");

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new QueryParameter("@p0", "Adam") };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenStringNotEqualTo_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 != "Adam");

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[String1] <> @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenStringNotEqualTo_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 != "Adam");

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new QueryParameter("@p0", "Adam") };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenStringStaticField_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == string.Empty);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[String1] = @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenStringStaticField_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == string.Empty);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new QueryParameter("@p0", string.Empty) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenStringIsNull_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == null);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[String1] is null";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenStringIsNull_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == null);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            Assert.AreEqual(0, query.Parameters.Count);
        }

        [Test]
        public void Process_WhenStringIsNotNull_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 != null);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[String1] is not null";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenStringIsNotNull_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 != null);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            Assert.AreEqual(0, query.Parameters.Count);
        }

        [Test]
        public void Process_WhenStringField_GeneratesCorrectSqlQuery()
        {
            var value = "Adam";
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == value);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[String1] = @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenStringField_ExtractsCorrectParameters()
        {
            var value = "Adam";
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == value);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new QueryParameter("@p0", value) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenStringConst_GeneratesCorrectSqlQuery()
        {
            const string value = "Adam";
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == value);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[String1] = @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenStringConst_ExtractsCorrectParameters()
        {
            const string value = "Adam";
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == value);

            var processor = new ParsedSelectorLambdaSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new QueryParameter("@p0", value) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }
    }
}