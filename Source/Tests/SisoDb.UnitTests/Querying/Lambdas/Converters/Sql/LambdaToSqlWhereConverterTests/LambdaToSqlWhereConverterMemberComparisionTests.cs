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

            Assert.AreEqual(
                "(si.[MemberPath]='Int1' and si.[IntegerValue] = (select sub.[IntegerValue] from [TempIndexes] sub where sub.[MemberPath]='Int2'))",
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