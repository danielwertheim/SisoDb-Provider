using System;
using System.Linq;
using NUnit.Framework;
using SisoDb.Dac;
using SisoDb.Querying.Lambdas.Converters.Sql;
using SisoDb.UnitTests.TestFactories;

namespace SisoDb.UnitTests.Querying.Lambdas.Converters.Sql.LambdaToSqlWhereConverterTests
{
    [TestFixture]
    public class LambdaToSqlWhereConverterCommonDataTypeTests : LambdaToSqlWhereConverterTestBase
    {
        [Test]
        [Ignore] //Until PineCone indexes StructureId as well
        public void Process_WhenMemberNameIsId_SqlQueryHasTranslatedId()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.StructureId == 42);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "StructureId"));
            Assert.AreEqual("si.[StructureId] = @p0", query.CriteriaString);
        }

        [Test]
        [Ignore] //Until PineCone indexes StructureId as well
        public void Process_WhenMemberNameIsId_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.StructureId == 42);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", 42) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenNestedMemberNameIsId_SqlQueryDoesNotTranslateId()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.NestedItem.StructureId == 42);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "NestedItem.StructureId"));
            Assert.AreEqual("(mem0.[IntegerValue] = @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenNestedMemberNameIsId_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.NestedItem.StructureId == 42);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", 42) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenInt_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 == 42);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "Int1"));
            Assert.AreEqual("(mem0.[IntegerValue] = @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenInt_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 == 42);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", 42) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenNullableValueTypeComparedAgainstVariable_GeneratesCorrectSqlQuery()
        {
            var value = 42;
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.NullableInt == value);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "NullableInt"));
            Assert.AreEqual("(mem0.[IntegerValue] = @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenNullableValueTypeComparedAgainstVariable_ExtractsCorrectParameters()
        {
            var value = 42;
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.NullableInt == value);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", 42) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenNegativeInt_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 == -42);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "Int1"));
            Assert.AreEqual("(mem0.[IntegerValue] = @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenNegativeInt_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 == -42);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", -42) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenIntNotEqualTo_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 != 42);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "Int1"));
            Assert.AreEqual("(mem0.[IntegerValue] <> @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenIntNotEqualTo_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 != 42);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", 42) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenIntGreaterThanEqualTo_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 >= 42);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "Int1"));
            Assert.AreEqual("(mem0.[IntegerValue] >= @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenIntGreaterThanEqualTo_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 >= 42);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", 42) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenIntLowerThanEqualTo_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 <= 42);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "Int1"));
            Assert.AreEqual("(mem0.[IntegerValue] <= @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenIntLowerThanEqualTo_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 <= 42);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", 42) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenIntGreaterThan_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 > 42);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "Int1"));
            Assert.AreEqual("(mem0.[IntegerValue] > @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenIntGreaterThan_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 > 42);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", 42) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenIntLowerThan_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 < 42);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "Int1"));
            Assert.AreEqual("(mem0.[IntegerValue] < @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenIntLowerThan_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 < 42);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", 42) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenDecimal_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Decimal1 == 3.14M);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "Decimal1"));
            Assert.AreEqual("(mem0.[FractalValue] = @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenDecimal_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Decimal1 == 3.14M);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", 3.14M) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenFalseBool_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Bool1 == false);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "Bool1"));
            Assert.AreEqual("(mem0.[BoolValue] = @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenFalseBool_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Bool1 == false);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", false) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenTrueBool_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Bool1 == true);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "Bool1"));
            Assert.AreEqual("(mem0.[BoolValue] = @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenTrueBool_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Bool1 == true);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", true) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenNotEqualToTrueBool_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Bool1 != true);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "Bool1"));
            Assert.AreEqual("(mem0.[BoolValue] <> @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenNotEqualToTrueBool_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Bool1 != true);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", true) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenNotEqualToFalseBool_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Bool1 != false);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "Bool1"));
            Assert.AreEqual("(mem0.[BoolValue] <> @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenNotEqualToFalseBool_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Bool1 != false);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", false) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenGuidField_GeneratesCorrectSqlQuery()
        {
            var guid = Guid.Empty;
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Guid1 == guid);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "Guid1"));
            Assert.AreEqual("(mem0.[GuidValue] = @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenGuidField_ExtractsCorrectParameters()
        {
            var guid = Guid.Empty;
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Guid1 == guid);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", guid) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenGuidStaticField_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Guid1 == Guid.Empty);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "Guid1"));
            Assert.AreEqual("(mem0.[GuidValue] = @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenGuidStaticField_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Guid1 == Guid.Empty);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", Guid.Empty) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenDateTimeField_GeneratesCorrectSqlQuery()
        {
            var dateTime = new DateTime(2010, 2, 3, 12, 13, 14);
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.DateTime1 == dateTime);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "DateTime1"));
            Assert.AreEqual("(mem0.[DateTimeValue] = @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenDateTimeField_ExtractsCorrectParameters()
        {
            var dateTime = new DateTime(2010, 2, 3, 12, 13, 14);
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.DateTime1 == dateTime);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", dateTime) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenDateTimeStaticProperty_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.DateTime1 == DateTime.Now);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "DateTime1"));
            Assert.AreEqual("(mem0.[DateTimeValue] = @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenDateTimeStaticProperty_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.DateTime1 == DateTime.Now);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameter = new DacParameter("@p0", DateTime.Now);
            var actualParameter = query.Parameters.Single();
            var dateTimeNowDelta = DateTime.Now.Subtract((DateTime)actualParameter.Value);
            Assert.AreEqual(expectedParameter.Name, actualParameter.Name);
            Assert.AreEqual(0, dateTimeNowDelta.TotalSeconds, 1);
        }

        [Test]
        public void Process_WhenString_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == "Adam");

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "String1"));
            Assert.AreEqual("(mem0.[StringValue] = @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenString_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == "Adam");

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", "Adam") };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenStringNotEqualTo_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 != "Adam");

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "String1"));
            Assert.AreEqual("(mem0.[StringValue] <> @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenStringNotEqualTo_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 != "Adam");

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", "Adam") };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenStringStaticField_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == string.Empty);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "String1"));
            Assert.AreEqual("(mem0.[StringValue] = @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenStringStaticField_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == string.Empty);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", string.Empty) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenStringIsNull_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == null);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "String1"));
            Assert.AreEqual("(mem0.[StringValue] is null)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenStringIsNull_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == null);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(0, query.Parameters.Count);
        }

        [Test]
        public void Process_WhenStringIsNotNull_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 != null);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "String1"));
            Assert.AreEqual("(mem0.[StringValue] is not null)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenStringIsNotNull_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 != null);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(0, query.Parameters.Count);
        }

        [Test]
        public void Process_WhenStringField_GeneratesCorrectSqlQuery()
        {
            var value = "Adam";
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == value);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "String1"));
            Assert.AreEqual("(mem0.[StringValue] = @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenStringField_ExtractsCorrectParameters()
        {
            var value = "Adam";
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == value);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", value) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void Process_WhenStringConst_GeneratesCorrectSqlQuery()
        {
            const string value = "Adam";
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == value);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "String1"));
            Assert.AreEqual("(mem0.[StringValue] = @p0)", query.CriteriaString);
        }

        [Test]
        public void Process_WhenStringConst_ExtractsCorrectParameters()
        {
            const string value = "Adam";
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.String1 == value);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", value) };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }
    }
}