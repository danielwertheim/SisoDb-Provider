using System.Linq;
using NUnit.Framework;
using SisoDb.Dac;
using SisoDb.Querying.Lambdas.Converters.Sql;
using SisoDb.UnitTests.TestFactories;

namespace SisoDb.UnitTests.Querying.Lambdas.Converters.Sql.LambdaToSqlWhereConverterTests
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

            Assert.AreEqual(2, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "Int1"));
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "String1"));
            Assert.AreEqual(
                "((((mem0.[IntegerValue] = @p0) and (mem1.[StringValue] = @p1)) or ((mem0.[IntegerValue] = @p2) and (mem1.[StringValue] = @p3))) or (mem0.[IntegerValue] = @p4))",
                query.CriteriaString);
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