using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.TestUtils;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008.UnitOfWork.Queries
{
    [TestFixture]
    public class SqlUnitOfWorkWhereTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<SimpleItemForQueries>();
        }

        [Test]
        public void Where_ByUsingInsertedItemInExpression_ReturnsMatchingItem()
        {
            var item = new SimpleItemForQueries { SortOrder = 1, StringValue = "A" };
            SimpleItemForQueries refetched = null;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();

                refetched = uow.Where<SimpleItemForQueries>(
                    x => x.SortOrder == item.SortOrder).SingleOrDefault();
            }

            Assert.IsNotNull(refetched);
        }

        [Test]
        public void WhereIntByInterval_WhenInRange_ReturnsMatchingItems()
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
                refetched = unitOfWork.Where<SimpleItemForQueries>(
                    i => i.SortOrder >= 3 && i.SortOrder <= 6).ToList();
            }

            Assert.AreEqual(4, refetched.Count());
            CustomAssert.AreValueEqual(items[2], refetched[0]);
            CustomAssert.AreValueEqual(items[5], refetched[3]);
        }

        [Test]
        public void Where_WhenQueryYieldsNoResult_NonNullEmptyResult()
        {
            IList<SimpleItemForQueries> refetched;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                refetched = unitOfWork.Where<SimpleItemForQueries>(
                    i => i.SortOrder < 1).ToList();
            }

            Assert.IsNotNull(refetched);
            Assert.AreEqual(0, refetched.Count());
        }

        [Test]
        public void WhereAsJsonIntByInterval_WhenInRange_ReturnsMatchingItems()
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
                refetched = unitOfWork.WhereAsJson<SimpleItemForQueries>(
                    i => i.SortOrder >= 3 && i.SortOrder <= 6).ToList();
            }

            Assert.AreEqual(4, refetched.Count());
            Assert.AreEqual("{\"SisoId\":3,\"SortOrder\":3}", refetched[0]);
            Assert.AreEqual("{\"SisoId\":6,\"SortOrder\":6}", refetched[3]);
        }

        [Test]
        public void WhereAsJson_WhenQueryYieldsNoResult_NonNullEmptyResult()
        {
            IList<string> refetched;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                refetched = unitOfWork.WhereAsJson<SimpleItemForQueries>(
                    i => i.SortOrder < 1).ToList();
            }

            Assert.IsNotNull(refetched);
            Assert.AreEqual(0, refetched.Count());
        }

        [Test]
        public void WhereAs_StoredAsXReturnAsY_ItemReturnedGetsPopulated()
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
                refetched = unitOfWork.WhereAs<SimpleItemForQueries, SimpleItemForQueriesInfo>(
                    i => i.SisoId != 0).ToList();
            }

            refetched.OrderBy(c => c.SortOrder);
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
            public int SisoId { get; set; }

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