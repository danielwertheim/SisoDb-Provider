using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.TestUtils;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.Queries
{
    [TestFixture]
    public class SqlUnitOfWorkQueryTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<ItemForQueries>();
        }

        [Test]
        public void Query_ByUsingInsertedItemInExpression_ReturnsMatchingItem()
        {
            var item = new ItemForQueries {SortOrder = 1, StringValue = "A"};
            ItemForQueries refetched = null;

            using(var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();

                refetched = uow.Query<ItemForQueries>(x => x.SortOrder == item.SortOrder).SingleOrDefault();
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

            IList<ItemForQueries> refetched;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                refetched = unitOfWork.Query<ItemForQueries>(i => i.SortOrder >= 3 && i.SortOrder <= 6)
                    .ToList();
            }

            Assert.AreEqual(4, refetched.Count());
            CustomAssert.AreValueEqual(items[2], refetched[0]);
            CustomAssert.AreValueEqual(items[5], refetched[3]);
        }

        [Test]
        public void Query_WhenQueryYieldsNoResult_NonNullEmptyResult()
        {
            IList<ItemForQueries> refetched;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                refetched = unitOfWork.Query<ItemForQueries>(i => i.SortOrder < 1)
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
                refetched = unitOfWork.QueryAsJson<ItemForQueries>(i => i.SortOrder >= 3 && i.SortOrder <= 6)
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
                refetched = unitOfWork.QueryAsJson<ItemForQueries>(i => i.SortOrder < 1)
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

            IList<ItemForQueriesInfo> refetched;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                refetched = unitOfWork.QueryAs<ItemForQueries, ItemForQueriesInfo>(i => i.Id != 0)
                    .OrderBy(c => c.SortOrder)
                    .ToList();
            }

            Assert.AreEqual(2, refetched.Count());
            Assert.AreEqual(items[0].SortOrder, refetched[0].SortOrder);
            Assert.AreEqual(items[0].StringValue, refetched[0].StringValue);
            Assert.AreEqual(items[1].SortOrder, refetched[1].SortOrder);
            Assert.AreEqual(items[1].StringValue, refetched[1].StringValue);
        }

        [Test]
        public void Query_InsertInOneOrderReturnedInAnother_SortingIsCorrect()
        {
            var items = new List<ItemForQueries>
                            {
                                new ItemForQueries {SortOrder = 1, StringValue = "A"},
                                new ItemForQueries {SortOrder = 5, StringValue = "E"},
                                new ItemForQueries {SortOrder = 2, StringValue = "B"},
                                new ItemForQueries {SortOrder = 4, StringValue = "D"},
                                new ItemForQueries {SortOrder = 3, StringValue = "C"}
                            };

            IList<ItemForQueries> refetched = null;
            using(var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                refetched = uow.Query<ItemForQueries>(q => q.SortBy(i => i.SortOrder)).ToList();
            }

            CustomAssert.AreValueEqual(items[0], refetched[0]);
            CustomAssert.AreValueEqual(items[1], refetched[4]);
            CustomAssert.AreValueEqual(items[2], refetched[1]);
            CustomAssert.AreValueEqual(items[3], refetched[3]);
            CustomAssert.AreValueEqual(items[4], refetched[2]);
        }

        [Test]
        public void QueryAs_InsertInOneOrderReturnedInAnother_SortingIsCorrect()
        {
            var items = new List<ItemForQueries>
                            {
                                new ItemForQueries {SortOrder = 1, StringValue = "A"},
                                new ItemForQueries {SortOrder = 5, StringValue = "E"},
                                new ItemForQueries {SortOrder = 2, StringValue = "B"},
                                new ItemForQueries {SortOrder = 4, StringValue = "D"},
                                new ItemForQueries {SortOrder = 3, StringValue = "C"}
                            };

            IList<ItemForQueriesInfo> refetched = null;
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                refetched = uow.QueryAs<ItemForQueries, ItemForQueriesInfo>(q => q.SortBy(i => i.StringValue)).ToList();
            }

            Assert.AreEqual(items[0].SortOrder, refetched[0].SortOrder);
            Assert.AreEqual(items[1].SortOrder, refetched[4].SortOrder);
            Assert.AreEqual(items[2].SortOrder, refetched[1].SortOrder);
            Assert.AreEqual(items[3].SortOrder, refetched[3].SortOrder);
            Assert.AreEqual(items[4].SortOrder, refetched[2].SortOrder);
        }

        [Test]
        public void QueryAsJson_InsertInOneOrderReturnedInAnother_SortingIsCorrect()
        {
            var items = new List<ItemForQueries>
                            {
                                new ItemForQueries {SortOrder = 1, StringValue = "A"},
                                new ItemForQueries {SortOrder = 5, StringValue = "E"},
                                new ItemForQueries {SortOrder = 2, StringValue = "B"},
                                new ItemForQueries {SortOrder = 4, StringValue = "D"},
                                new ItemForQueries {SortOrder = 3, StringValue = "C"}
                            };

            IList<string> refetched = null;
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                refetched = uow.QueryAsJson<ItemForQueries>(q => q.SortBy(i => i.SortOrder)).ToList();
            }

            Assert.AreEqual("{\"Id\":1,\"SortOrder\":1,\"StringValue\":\"A\"}", refetched[0]);
            Assert.AreEqual("{\"Id\":2,\"SortOrder\":5,\"StringValue\":\"E\"}", refetched[4]);
            Assert.AreEqual("{\"Id\":3,\"SortOrder\":2,\"StringValue\":\"B\"}", refetched[1]);
            Assert.AreEqual("{\"Id\":4,\"SortOrder\":4,\"StringValue\":\"D\"}", refetched[3]);
            Assert.AreEqual("{\"Id\":5,\"SortOrder\":3,\"StringValue\":\"C\"}", refetched[2]);
        }

        [Test]
        public void Query_InsertInOneOrderReturnedInAnotherBySortingOnTwoMembers_SortingIsCorrect()
        {
            var items = new List<ItemForQueries>
                            {
                                new ItemForQueries {SortOrder = 2, StringValue = "B"},
                                new ItemForQueries {SortOrder = 2, StringValue = "A"},
                                new ItemForQueries {SortOrder = 1, StringValue = "B"},
                                new ItemForQueries {SortOrder = 1, StringValue = "A"}
                            };

            IList<ItemForQueries> refetched = null;
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                refetched = uow.Query<ItemForQueries>(q => q.SortBy(i => i.StringValue, i => i.SortOrder)).ToList();
            }

            CustomAssert.AreValueEqual(items[0], refetched[3]);
            CustomAssert.AreValueEqual(items[1], refetched[1]);
            CustomAssert.AreValueEqual(items[2], refetched[2]);
            CustomAssert.AreValueEqual(items[3], refetched[0]);
        }


        private static IList<ItemForQueries> CreateItems(int numOfItems)
        {
            return ListExtensions.New<ItemForQueries>(numOfItems, (index, item) => item.SortOrder = index + 1);
        }

        private class ItemForQueries
        {
            public int Id { get; set; }

            public int SortOrder { get; set; }

            public string StringValue { get; set; }
        }

        private class ItemForQueriesInfo
        {
            public int SortOrder { get; set; }

            public string StringValue { get; set; }
        }
    }
}