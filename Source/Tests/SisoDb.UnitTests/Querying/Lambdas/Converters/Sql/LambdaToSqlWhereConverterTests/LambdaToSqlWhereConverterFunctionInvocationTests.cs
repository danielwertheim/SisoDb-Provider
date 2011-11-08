using System;
using NUnit.Framework;
using SisoDb.Dac;
using SisoDb.Querying.Lambdas.Converters.Sql;
using SisoDb.UnitTests.TestFactories;

namespace SisoDb.UnitTests.Querying.Lambdas.Converters.Sql.LambdaToSqlWhereConverterTests
{
    [TestFixture]
    public class LambdaToSqlWhereConverterFunctionInvocationTests : LambdaToSqlWhereConverterTestBase
    {
        [Test]
        public void Process_WhenStaticFunction_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.DateTime1 == DateTime.Parse("2010-11-12"));

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual("(si.[MemberPath]='DateTime1' and si.[DateTimeValue] = @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenStaticFunction_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.DateTime1 == DateTime.Parse("2010-11-12"));

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", new DateTime(2010, 11, 12)) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenStaticFunctionWithMultiArgs_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.DateTime1 == DateTime.Parse("2010-11-12", SisoEnvironment.Formatting.DateTimeFormatProvider));

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual("(si.[MemberPath]='DateTime1' and si.[DateTimeValue] = @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenStaticFunctionWithMultiArgs_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.DateTime1 == DateTime.Parse("2010-11-12", SisoEnvironment.Formatting.DateTimeFormatProvider));

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", new DateTime(2010, 11, 12)) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenInstanceFunction_GeneratesCorrectSqlQuery()
        {
            var dateTime = new DateTime(2010, 11, 12);
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == dateTime.ToString(SisoEnvironment.Formatting.DateTimeFormatProvider));

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual("(si.[MemberPath]='String1' and si.[StringValue] = @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenInstanceFunction_ExtractsCorrectParameters()
        {
            var dateTime = new DateTime(2010, 11, 12);
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == dateTime.ToString(SisoEnvironment.Formatting.DateTimeFormatProvider));

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", "2010-11-12 00:00:00") };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }
    }
}