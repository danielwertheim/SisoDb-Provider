using System.Linq;
using NUnit.Framework;
using SisoDb.Querying.Lambdas.Converters.Sql;
using SisoDb.UnitTests.TestFactories;

namespace SisoDb.UnitTests.Querying.Lambdas.Converters.Sql.LambdaToSqlWhereConverterTests
{
    [TestFixture]
    public class LambdaToSqlWhereConverterMemberComparisionTests : LambdaToSqlWhereConverterTestBase
    {
        [Test]
        public void Process_WhenMemberOfSameType_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 == i.Int2);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub(), parsedLambda);

            Assert.AreEqual(2, query.MemberPaths.Length);
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "Int1"));
            Assert.IsNotNull(query.MemberPaths.SingleOrDefault(m => m == "Int2"));
            Assert.AreEqual(
                "(mem0.[IntegerValue] = (mem1.[IntegerValue]))",
                query.CriteriaString);
        }

        [Test]
        public void Process_WhenMemberOfSameType_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1 == i.Int2);

            var processor = new LambdaToSqlWhereConverter();
            var query = processor.Convert(StructureSchemaTestFactory.Stub(), parsedLambda);

            Assert.AreEqual(0, query.Parameters.Count());
        }
    }
}