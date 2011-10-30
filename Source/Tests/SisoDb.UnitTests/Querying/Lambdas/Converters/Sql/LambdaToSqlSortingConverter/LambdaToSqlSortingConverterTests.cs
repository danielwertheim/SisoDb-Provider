using NUnit.Framework;
using SisoDb.Querying;
using SisoDb.UnitTests.TestFactories;

namespace SisoDb.UnitTests.Querying.Lambdas.Converters.Sql.LambdaToSqlSortingConverter
{
    [TestFixture]
    public class LambdaToSqlSortingConverterTests : LambdaToSqlSortingConverterTestBase
    {
        [Test]
        public void Process_WhenMemberNameIsId_SqlTranslatesToStructureId()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.StructureId);

            var processor = new SisoDb.Querying.Lambdas.Converters.Sql.LambdaToSqlSortingConverter();
            var sorting = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(string.Empty, sorting.SortingJoins);
            Assert.AreEqual("s.[StructureId] Asc", sorting.Sorting);
        }

        [Test]
        public void Process_WhenOneMemberSortingImplicitAscending_SqlForOneMemberAscending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1);

            var processor = new SisoDb.Querying.Lambdas.Converters.Sql.LambdaToSqlSortingConverter();
            var sorting = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual("(si.[MemberPath]='Int1')", sorting.SortingJoins);
            Assert.AreEqual("min(si.[IntegerValue]) Asc", sorting.Sorting);
        }

        [Test]
        public void Process_WhenOneNestedMemberSortingImplicitAscending_SqlForOneMemberAscending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.NestedItem.SuperNestedItem.Int1);

            var processor = new SisoDb.Querying.Lambdas.Converters.Sql.LambdaToSqlSortingConverter();
            var sorting = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual("(si.[MemberPath]='NestedItem.SuperNestedItem.Int1')", sorting.SortingJoins);
            Assert.AreEqual("min(si.[IntegerValue]) Asc", sorting.Sorting);
        }

        [Test]
        public void Process_WhenOneMemberSortingExplicitAscending_SqlWithOneMemberAscending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1.Asc());

            var processor = new SisoDb.Querying.Lambdas.Converters.Sql.LambdaToSqlSortingConverter();
            var sorting = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual("(si.[MemberPath]='Int1')", sorting.SortingJoins);
            Assert.AreEqual("min(si.[IntegerValue]) Asc", sorting.Sorting);
        }

        [Test]
        public void Process_WhenOneMemberSortingExplicitDescending_SqlWithOneMemberDescending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1.Desc());

            var processor = new SisoDb.Querying.Lambdas.Converters.Sql.LambdaToSqlSortingConverter();
            var sorting = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual("(si.[MemberPath]='Int1')", sorting.SortingJoins);
            Assert.AreEqual("min(si.[IntegerValue]) Desc", sorting.Sorting);
        }

        [Test]
        public void Process_WhenTwoMembersWhereOneIsImplicitAscendingAndOneExplicitDescending_SqlWithOneMemberAscendingAndOneDescending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1.Desc(), i => i.DateTime1);

            var processor = new SisoDb.Querying.Lambdas.Converters.Sql.LambdaToSqlSortingConverter();
            var sorting = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual("(si.[MemberPath]='Int1' or si.[MemberPath]='DateTime1')", sorting.SortingJoins);
            Assert.AreEqual("min(si.[IntegerValue]) Desc, min(si.[DateTimeValue]) Asc", sorting.Sorting);
        }
    }
}