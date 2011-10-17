using NUnit.Framework;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Converters.Sql;
using SisoDb.UnitTests.TestFactories;

namespace SisoDb.UnitTests.Querying.Lambdas.Converters.Sql.LambdaToSqlSortingConverterTests
{
    [TestFixture]
    public class LambdaToSqlSortingConverterTests : LambdaToSqlSortingConverterTestBase
    {
        [Test]
        public void Process_WhenMemberNameIsId_SqlTranslatesToStructureId()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.StructureId);

            var processor = new LambdaToSqlSortingConverter();
            var sorting = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(string.Empty, sorting.SortingJoins);
            Assert.AreEqual("s.[StructureId] Asc", sorting.Sorting);
        }

        [Test]
        public void Process_WhenOneMemberSortingImplicitAscending_SqlForOneMemberAscending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1);

            var processor = new LambdaToSqlSortingConverter();
            var sorting = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(
                "left join [dbo].[MyItemIndexes] as siSort1 on siSort1.[StructureId] = s.[StructureId] and siSort1.[MemberPath]='Int1'",
                sorting.SortingJoins);
            Assert.AreEqual("siSort1.[IntegerValue] Asc", sorting.Sorting);
        }

        [Test]
        public void Process_WhenOneNestedMemberSortingImplicitAscending_SqlForOneMemberAscending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.NestedItem.SuperNestedItem.Int1);

            var processor = new LambdaToSqlSortingConverter();
            var sorting = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(
                "left join [dbo].[MyItemIndexes] as siSort1 on siSort1.[StructureId] = s.[StructureId] and siSort1.[MemberPath]='NestedItem.SuperNestedItem.Int1'",
                sorting.SortingJoins);
            Assert.AreEqual("siSort1.[IntegerValue] Asc", sorting.Sorting);
        }

        [Test]
        public void Process_WhenOneMemberSortingExplicitAscending_SqlWithOneMemberAscending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1.Asc());

            var processor = new LambdaToSqlSortingConverter();
            var sorting = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(
                "left join [dbo].[MyItemIndexes] as siSort1 on siSort1.[StructureId] = s.[StructureId] and siSort1.[MemberPath]='Int1'",
                sorting.SortingJoins);
            Assert.AreEqual("siSort1.[IntegerValue] Asc", sorting.Sorting);
        }

        [Test]
        public void Process_WhenOneMemberSortingExplicitDescending_SqlWithOneMemberDescending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1.Desc());

            var processor = new LambdaToSqlSortingConverter();
            var sorting = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(
                "left join [dbo].[MyItemIndexes] as siSort1 on siSort1.[StructureId] = s.[StructureId] and siSort1.[MemberPath]='Int1'",
                sorting.SortingJoins);
            Assert.AreEqual("siSort1.[IntegerValue] Desc", sorting.Sorting);
        }

        [Test]
        public void Process_WhenTwoMembersWhereOneIsImplicitAscendingAndOneExplicitDescending_SqlWithOneMemberAscendingAndOneDescending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1.Desc(), i => i.DateTime1);

            var processor = new LambdaToSqlSortingConverter();
            var sorting = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(
                "left join [dbo].[MyItemIndexes] as siSort1 on siSort1.[StructureId] = s.[StructureId] and siSort1.[MemberPath]='Int1' " +
                "left join [dbo].[MyItemIndexes] as siSort2 on siSort2.[StructureId] = s.[StructureId] and siSort2.[MemberPath]='DateTime1'",
                sorting.SortingJoins);
            Assert.AreEqual("siSort1.[IntegerValue] Desc, siSort2.[DateTimeValue] Asc", sorting.Sorting);
        }
    }
}