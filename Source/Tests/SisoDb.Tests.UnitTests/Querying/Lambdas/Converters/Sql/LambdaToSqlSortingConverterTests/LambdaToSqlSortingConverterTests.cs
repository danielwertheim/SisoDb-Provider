using NUnit.Framework;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Converters.Sql;
using SisoDb.Tests.UnitTests.TestFactories;

namespace SisoDb.Tests.UnitTests.Querying.Lambdas.Converters.Sql.LambdaToSqlSortingConverterTests
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

            Assert.AreEqual("si.[StructureId] Asc", sorting.Sql);
        }

        [Test]
        public void Process_WhenOneMemberSortingImplicitAscending_SqlForOneMemberAscending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1);

            var processor = new LambdaToSqlSortingConverter();
            var sorting = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual("si.[Int1] Asc", sorting.Sql);
        }

        [Test]
        public void Process_WhenOneNestedMemberSortingImplicitAscending_SqlForOneMemberAscending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.NestedItem.SuperNestedItem.Int1);

            var processor = new LambdaToSqlSortingConverter();
            var sorting = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual("si.[NestedItem.SuperNestedItem.Int1] Asc", sorting.Sql);
        }

        [Test]
        public void Process_WhenOneMemberSortingExplicitAscending_SqlWithOneMemberAscending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1.Asc());

            var processor = new LambdaToSqlSortingConverter();
            var sorting = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual("si.[Int1] Asc", sorting.Sql);
        }

        [Test]
        public void Process_WhenOneMemberSortingExplicitDescending_SqlWithOneMemberDescending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1.Desc());

            var processor = new LambdaToSqlSortingConverter();
            var sorting = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual("si.[Int1] Desc", sorting.Sql);
        }

        [Test]
        public void Process_WhenTwoMembersWhereOneIsImplicitAscendingAndOneExplicitDescending_SqlWithOneMemberAscendingAndOneDescending()
        {
            var parsedLambda = CreateParsedLambda<MyItem>(i => i.Int1.Desc(), i => i.DateTime1);

            var processor = new LambdaToSqlSortingConverter();
            var sorting = processor.Convert(StructureSchemaTestFactory.Stub<MyItem>(), parsedLambda);

            Assert.AreEqual("si.[Int1] Desc, si.[DateTime1] Asc", sorting.Sql);
        }
    }
}