using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.TestUtils;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.Queries
{
    [TestFixture]
    public class SqlUnitOfWorkSimpleQueryTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<SimpleItemForQueries>();
        }

        [Test]
        public void Query_ByUsingInsertedItemInExpression_ReturnsMatchingItem()
        {
            var item = new SimpleItemForQueries { SortOrder = 1, StringValue = "A" };
            SimpleItemForQueries refetched = null;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();

                refetched = uow.SimpleQuery<SimpleItemForQueries>(x => x.SortOrder == item.SortOrder).SingleOrDefault();
            }

            Assert.IsNotNull(refetched);
        }

        [Test]
        public void QueryIntByInterval_WhenInRange_ReturnsMatchingItems()
        {
            var items = CreateItems(10);
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();
            }

            IList<SimpleItemForQueries> refetched;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                refetched = unitOfWork.SimpleQuery<SimpleItemForQueries>(i => i.SortOrder >= 3 && i.SortOrder <= 6)
                    .ToList();
            }

            Assert.AreEqual(4, refetched.Count());
            CustomAssert.AreValueEqual(items[2], refetched[0]);
            CustomAssert.AreValueEqual(items[5], refetched[3]);
        }

        [Test]
        public void Query_WhenQueryYieldsNoResult_NonNullEmptyResult()
        {
            IList<SimpleItemForQueries> refetched;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                refetched = unitOfWork.SimpleQuery<SimpleItemForQueries>(i => i.SortOrder < 1)
                    .ToList();
            }

            Assert.IsNotNull(refetched);
            Assert.AreEqual(0, refetched.Count());
        }

        [Test]
        public void QueryAsJsonIntByInterval_WhenInRange_ReturnsMatchingItems()
        {
            var items = CreateItems(10);
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();
            }

            IList<string> refetched;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                refetched = unitOfWork.SimpleQueryAsJson<SimpleItemForQueries>(i => i.SortOrder >= 3 && i.SortOrder <= 6)
                    .ToList();
            }

            Assert.AreEqual(4, refetched.Count());
            Assert.AreEqual("{\"Id\":3,\"SortOrder\":3}", refetched[0]);
            Assert.AreEqual("{\"Id\":6,\"SortOrder\":6}", refetched[3]);
        }

        [Test]
        public void QueryAsJson_WhenQueryYieldsNoResult_NonNullEmptyResult()
        {
            IList<string> refetched;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                refetched = unitOfWork.SimpleQueryAsJson<SimpleItemForQueries>(i => i.SortOrder < 1)
                    .ToList();
            }

            Assert.IsNotNull(refetched);
            Assert.AreEqual(0, refetched.Count());
        }

        [Test]
        public void QueryAs_StoredAsXReturnAsY_ItemReturnedGetsPopulated()
        {
            var items = CreateItems(2);
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();
            }

            IList<SimpleItemForQueriesInfo> refetched;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                refetched = unitOfWork.SimpleQueryAs<SimpleItemForQueries, SimpleItemForQueriesInfo>
                    (i => i.Id != 0).OrderBy(c => c.SortOrder).ToList();
            }

            Assert.AreEqual(2, refetched.Count());
            Assert.AreEqual(items[0].SortOrder, refetched[0].SortOrder);
            Assert.AreEqual(items[0].StringValue, refetched[0].StringValue);
            Assert.AreEqual(items[1].SortOrder, refetched[1].SortOrder);
            Assert.AreEqual(items[1].StringValue, refetched[1].StringValue);
        }

        private static IList<SimpleItemForQueries> CreateItems(int numOfItems)
        {
            return ListExtensions.New<SimpleItemForQueries>(numOfItems, (index, item) => item.SortOrder = index + 1);
        }

        private class SimpleItemForQueries
        {
            public int Id { get; set; }

            public int SortOrder { get; set; }

            public string StringValue { get; set; }
        }

        private class SimpleItemForQueriesInfo
        {
            public int SortOrder { get; set; }

            public string StringValue { get; set; }
        }
    }
}