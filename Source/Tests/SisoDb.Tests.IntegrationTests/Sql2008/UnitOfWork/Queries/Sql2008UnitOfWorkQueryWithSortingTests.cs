using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests.Sql2008.UnitOfWork.Queries
{
    [TestFixture]
    public class Sql2008UnitOfWorkQueryWithSortingTests : Sql2008IntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<ItemForQueries>();
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

            Assert.AreEqual("{\"StructureId\":1,\"SortOrder\":1,\"StringValue\":\"A\"}", refetched[0]);
            Assert.AreEqual("{\"StructureId\":2,\"SortOrder\":5,\"StringValue\":\"E\"}", refetched[4]);
            Assert.AreEqual("{\"StructureId\":3,\"SortOrder\":2,\"StringValue\":\"B\"}", refetched[1]);
            Assert.AreEqual("{\"StructureId\":4,\"SortOrder\":4,\"StringValue\":\"D\"}", refetched[3]);
            Assert.AreEqual("{\"StructureId\":5,\"SortOrder\":3,\"StringValue\":\"C\"}", refetched[2]);
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

        private class ItemForQueries
        {
            public int StructureId { get; set; }

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