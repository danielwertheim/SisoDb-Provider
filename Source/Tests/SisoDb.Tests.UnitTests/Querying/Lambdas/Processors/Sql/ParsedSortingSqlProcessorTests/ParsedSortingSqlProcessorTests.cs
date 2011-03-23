using NUnit.Framework;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Processors.Sql;

namespace SisoDb.Tests.UnitTests.Querying.Lambdas.Processors.Sql.ParsedSortingSqlProcessorTests
{
    [TestFixture]
    public class ParsedSortingSqlProcessorTests : ParsedSortingSqlProcessorTestBase
    {
        [Test]
        public void Process_WhenMemberNameIsId_SqlTranslatesToStructureId()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Id);

            var processor = new ParsedSortingSqlProcessor(new MemberNameGeneratorFake());
            var sorting = processor.Process(parsedLambda);

            const string expectedSql = "si.[StructureId] Asc";
            Assert.AreEqual(expectedSql, sorting.Sql);
        }

        [Test]
        public void Process_WhenOneMemberSortingImplicitAscending_SqlForOneMemberAscending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1);

            var processor = new ParsedSortingSqlProcessor(new MemberNameGeneratorFake());
            var sorting = processor.Process(parsedLambda);

            const string expectedSql = "si.[Int1] Asc";
            Assert.AreEqual(expectedSql, sorting.Sql);
        }

        [Test]
        public void Process_WhenOneNestedMemberSortingImplicitAscending_SqlForOneMemberAscending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.NestedItem.SuperNestedItem.Int1);

            var processor = new ParsedSortingSqlProcessor(new MemberNameGeneratorFake());
            var sorting = processor.Process(parsedLambda);

            const string expectedSql = "si.[NestedItem.SuperNestedItem.Int1] Asc";
            Assert.AreEqual(expectedSql, sorting.Sql);
        }

        [Test]
        public void Process_WhenOneMemberSortingExplicitAscending_SqlWithOneMemberAscending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1.Asc());

            var processor = new ParsedSortingSqlProcessor(new MemberNameGeneratorFake());
            var sorting = processor.Process(parsedLambda);

            const string expectedSql = "si.[Int1] Asc";
            Assert.AreEqual(expectedSql, sorting.Sql);
        }

        [Test]
        public void Process_WhenOneMemberSortingExplicitDescending_SqlWithOneMemberDescending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1.Desc());

            var processor = new ParsedSortingSqlProcessor(new MemberNameGeneratorFake());
            var sorting = processor.Process(parsedLambda);

            const string expectedSql = "si.[Int1] Desc";
            Assert.AreEqual(expectedSql, sorting.Sql);
        }

        [Test]
        public void Process_WhenTwoMembersWhereOneIsImplicitAscendingAndOneExplicitDescending_SqlWithOneMemberAscendingAndOneDescending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1.Desc(), i => i.DateTime1);

            var processor = new ParsedSortingSqlProcessor(new MemberNameGeneratorFake());
            var sorting = processor.Process(parsedLambda);

            const string expectedSql = "si.[Int1] Desc, si.[DateTime1] Asc";
            Assert.AreEqual(expectedSql, sorting.Sql);
        }
    }
}