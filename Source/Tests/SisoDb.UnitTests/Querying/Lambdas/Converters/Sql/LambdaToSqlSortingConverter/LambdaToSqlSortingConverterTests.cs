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
            var sortings = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, sortings.Count);

            Assert.AreEqual("[StructureId]", sortings[0].IndexStorageColumnName);
            Assert.AreEqual("s.[StructureId]", sortings[0].Sorting);
            Assert.AreEqual("sort0", sortings[0].Alias);
            Assert.AreEqual("Asc", sortings[0].Direction);
        }

        [Test]
        public void Process_WhenOneMemberSortingImplicitAscending_SqlForOneMemberAscending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1);

            var processor = new SisoDb.Querying.Lambdas.Converters.Sql.LambdaToSqlSortingConverter();
            var sortings = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, sortings.Count);
            Assert.AreEqual("[IntegerValue]", sortings[0].IndexStorageColumnName);
            Assert.AreEqual("si.[IntegerValue]", sortings[0].Sorting);
            Assert.AreEqual("sort0", sortings[0].Alias);
            Assert.AreEqual("Asc", sortings[0].Direction);
        }

        [Test]
        public void Process_WhenOneNestedMemberSortingImplicitAscending_SqlForOneMemberAscending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.NestedItem.SuperNestedItem.Int1);

            var processor = new SisoDb.Querying.Lambdas.Converters.Sql.LambdaToSqlSortingConverter();
            var sortings = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, sortings.Count);
            Assert.AreEqual("[IntegerValue]", sortings[0].IndexStorageColumnName);
            Assert.AreEqual("si.[IntegerValue]", sortings[0].Sorting);
            Assert.AreEqual("sort0", sortings[0].Alias);
            Assert.AreEqual("Asc", sortings[0].Direction);
        }

        [Test]
        public void Process_WhenOneMemberSortingExplicitAscending_SqlWithOneMemberAscending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1.Asc());

            var processor = new SisoDb.Querying.Lambdas.Converters.Sql.LambdaToSqlSortingConverter();
            var sortings = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, sortings.Count);
            Assert.AreEqual("[IntegerValue]", sortings[0].IndexStorageColumnName);
            Assert.AreEqual("si.[IntegerValue]", sortings[0].Sorting);
            Assert.AreEqual("sort0", sortings[0].Alias);
            Assert.AreEqual("Asc", sortings[0].Direction);
        }

        [Test]
        public void Process_WhenOneMemberSortingExplicitDescending_SqlWithOneMemberDescending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1.Desc());

            var processor = new SisoDb.Querying.Lambdas.Converters.Sql.LambdaToSqlSortingConverter();
            var sortings = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(1, sortings.Count);
            Assert.AreEqual("[IntegerValue]", sortings[0].IndexStorageColumnName);
            Assert.AreEqual("si.[IntegerValue]", sortings[0].Sorting);
            Assert.AreEqual("sort0", sortings[0].Alias);
            Assert.AreEqual("Desc", sortings[0].Direction);
        }

        [Test]
        public void Process_WhenTwoMembersWhereOneIsImplicitAscendingAndOneExplicitDescending_SqlWithOneMemberAscendingAndOneDescending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1.Desc(), i => i.DateTime1);

            var processor = new SisoDb.Querying.Lambdas.Converters.Sql.LambdaToSqlSortingConverter();
            var sortings = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual(2, sortings.Count);

            Assert.AreEqual("[IntegerValue]", sortings[0].IndexStorageColumnName);
            Assert.AreEqual("si.[IntegerValue]", sortings[0].Sorting);
            Assert.AreEqual("sort0", sortings[0].Alias);
            Assert.AreEqual("Desc", sortings[0].Direction);

            Assert.AreEqual("[DateTimeValue]", sortings[1].IndexStorageColumnName);
            Assert.AreEqual("si.[DateTimeValue]", sortings[1].Sorting);
            Assert.AreEqual("sort1", sortings[1].Alias);
            Assert.AreEqual("Asc", sortings[1].Direction);
        }
    }
}