using NUnit.Framework;
using SisoDb.Dac;
using SisoDb.Querying.Lambdas.Converters.Sql;
using SisoDb.Tests.UnitTests.TestFactories;

namespace SisoDb.Tests.UnitTests.Querying.Lambdas.Converters.Sql.LambdaToSqlWhereConverterTests
{
    [TestFixture]
    public class LambdaToSqlWhereConverterCombinedTests : LambdaToSqlWhereConverterTestBase
    {
        [Test]
        public void Process_WhenCombinedMembers_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i =>
                (i.Int1 == 42 && i.String1 == "A") ||
                (i.Int1 == 11 && i.String1 == "AA") ||
                i.Int1 == 99);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            const string expectedSql = "((((si.[MemberPath]='Int1' and si.[IntegerValue] = @p0) and (si.[MemberPath]='String1' and si.[StringValue] = @p1)) or ((si.[MemberPath]='Int1' and si.[IntegerValue] = @p2) and (si.[MemberPath]='String1' and si.[StringValue] = @p3))) or (si.[MemberPath]='Int1' and si.[IntegerValue] = @p4))";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void Process_WhenCombinedMembers_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i =>
                (i.Int1 == 42 && i.String1 == "A") ||
                (i.Int1 == 11 && i.String1 == "AA") ||
                i.Int1 == 99);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

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