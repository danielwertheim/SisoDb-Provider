using NUnit.Framework;
using SisoDb.Dac;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Processors.Sql;

namespace SisoDb.Tests.UnitTests.Querying.Lambdas.Processors.Sql.ParsedWhereSqlProcessorTests
{
    [TestFixture]
    public class ParsedWhereSqlProcessorEnumerableQueryExtensionsTests : ParsedWhereSqlProcessorTestBase
    {
        [Test]
        public void QxAny_OnStrings_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Strings.QxAny(s => s == "Alpha" || s == "Bravo"));

            var processor = new ParsedWhereSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "(si.[Strings] like @p0 or si.[Strings] like @p1)";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void QxAny_OnStrings_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Strings.QxAny(s => s == "Alpha" || s == "Bravo"));

            var processor = new ParsedWhereSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", "%<$Alpha$>%"), new DacParameter("@p1", "%<$Bravo$>%") };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void QxAny_OnDecimals_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Decimals.QxAny(e => e == 3.14M || e == -1.89M));

            var processor = new ParsedWhereSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "(si.[Decimals] like @p0 or si.[Decimals] like @p1)";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void QxAny_OnDecimals_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Decimals.QxAny(e => e == 3.14M || e == -1.89M));

            var processor = new ParsedWhereSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", "%<$3.14$>%"), new DacParameter("@p1", "%<$-1.89$>%") };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void QxAny_OnListOfIntegers_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.ListOfIntegers.QxAny(v => v == 42));

            var processor = new ParsedWhereSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[ListOfIntegers] like @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void QxAny_OnListOfIntegers_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.ListOfIntegers.QxAny(v => v == 42));

            var processor = new ParsedWhereSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", "%<$42$>%") };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void QxAny_OnChildItems_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.ChildItems.QxAny(c => c.Int1 == 42 || c.Int1 == 55));

            var processor = new ParsedWhereSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "(si.[ChildItems.Int1] like @p0 or si.[ChildItems.Int1] like @p1)";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void QxAny_OnChildItems_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.ChildItems.QxAny(c => c.Int1 == 42 || c.Int1 == 55));

            var processor = new ParsedWhereSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", "%<$42$>%"), new DacParameter("@p1", "%<$55$>%") };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void QxAny_WhenNested_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.ChildItems.QxAny(c => c.GrandChildItems.QxAny(c2 => c2.Int1 == 42)));

            var processor = new ParsedWhereSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[ChildItems.GrandChildItems.Int1] like @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void QxAny_WhenNested_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.ChildItems.QxAny(c => c.GrandChildItems.QxAny(c2 => c2.Int1 == 42)));

            var processor = new ParsedWhereSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", "%<$42$>%") };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }

        [Test]
        public void QxAny_WhenValueArrayOnChildItem_GeneratesCorrectSqlQuery()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.ChildItems.QxAny(e => e.Integers.QxAny(e2 => e2 == 42)));

            var processor = new ParsedWhereSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            const string expectedSql = "si.[ChildItems.Integers] like @p0";
            Assert.AreEqual(expectedSql, query.Sql);
        }

        [Test]
        public void QxAny_WhenValueArrayOnChildItem_ExtractsCorrectParameters()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.ChildItems.QxAny(e => e.Integers.QxAny(e2 => e2 == 42)));

            var processor = new ParsedWhereSqlProcessor(new MemberNameGeneratorFake());
            var query = processor.Process(parsedLambda);

            var expectedParameters = new[] { new DacParameter("@p0", "%<$42$>%") };
            AssertQueryParameters(expectedParameters, query.Parameters);
        }
    }
}