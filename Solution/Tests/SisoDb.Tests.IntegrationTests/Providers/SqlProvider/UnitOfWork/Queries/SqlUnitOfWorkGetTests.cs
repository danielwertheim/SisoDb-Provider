using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.TestUtils;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.Queries
{
    [TestFixture]
    public class SqlUnitOfWorkGetTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<IdentityItemForGetQueries>();
            DropStructureSet<GuidItemForGetQueries>();
        }

        [Test]
        public void GetAll_WhenNoStructuresExists_ReturnsNull()
        {
            using(var unitOfWork = Database.CreateUnitOfWork())
            {
                var result = unitOfWork.GetAll<IdentityItemForGetQueries>();
                Assert.AreEqual(0, result.Count());
            }
        }

        [Test]
        public void GetAll_WhenTwoItemsExists_ReturnsBothItems()
        {
            var item1 = new IdentityItemForGetQueries { SortOrder = 1 };
            var item2 = new IdentityItemForGetQueries { SortOrder = 2 };

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                unitOfWork.Insert(item1);
                unitOfWork.Insert(item2);
                unitOfWork.Commit();
            }

            IEnumerable<IdentityItemForGetQueries> refetched;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                refetched = unitOfWork.GetAll<IdentityItemForGetQueries>().ToArray();
            }

            Assert.AreEqual(2, refetched.Count());
        }

        [Test]
        public void GetAllAsJson_WhenTwoItemsExists_ReturnsBothItems()
        {
            var item1 = new IdentityItemForGetQueries { SortOrder = 1 };
            var item2 = new IdentityItemForGetQueries { SortOrder = 2 };

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                unitOfWork.Insert(item1);
                unitOfWork.Insert(item2);
                unitOfWork.Commit();
            }

            IEnumerable<string> refetched;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                refetched = unitOfWork.GetAllAsJson<IdentityItemForGetQueries>().ToArray();
            }

            Assert.AreEqual(2, refetched.Count());
        }

        [Test]
        public void GetByIdentityId_WhenTwoItemsExistsAndTheSecondIsQueriedFor_ReturnsTheCorrectItem()
        {
            var item1 = new IdentityItemForGetQueries { SortOrder = 1 };
            var item2 = new IdentityItemForGetQueries { SortOrder = 2 };

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                unitOfWork.Insert(item1);
                unitOfWork.Insert(item2);
                unitOfWork.Commit();
            }

            IdentityItemForGetQueries itemRefetched;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                itemRefetched = unitOfWork.GetById<IdentityItemForGetQueries>(item2.SisoId);
            }

            Assert.AreEqual(item2.SortOrder, itemRefetched.SortOrder);
        }

        [Test]
        public void GetByGuidId_WhenTwoItemsExistsAndTheSecondIsQueriedFor_ReturnsTheCorrectItem()
        {
            var item1 = new GuidItemForGetQueries { SortOrder = 1 };
            var item2 = new GuidItemForGetQueries { SortOrder = 2 };

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                unitOfWork.Insert(item1);
                unitOfWork.Insert(item2);
                unitOfWork.Commit();
            }

            GuidItemForGetQueries itemRefetched;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                itemRefetched = unitOfWork.GetById<GuidItemForGetQueries>(item2.SisoId);
            }

            Assert.AreEqual(item2.SortOrder, itemRefetched.SortOrder);
        }

        [Test]
        public void GetByIdentityIdAsJson_WhenTwoItemsExistsAndTheSecondIsQueriedFor_ReturnsTheCorrectItemJson()
        {
            var item1 = new IdentityItemForGetQueries { SortOrder = 1 };
            var item2 = new IdentityItemForGetQueries { SortOrder = 2 };

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                unitOfWork.Insert(item1);
                unitOfWork.Insert(item2);
                unitOfWork.Commit();
            }

            string itemJsonRefetched;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                itemJsonRefetched = unitOfWork.GetByIdAsJson<IdentityItemForGetQueries>(item2.SisoId);
            }

            Assert.AreEqual("{\"SisoId\":2,\"SortOrder\":2}", itemJsonRefetched);
        }

        [Test]
        public void GetByIdentityIdAsJson_WhenItemDoesNotExist_ReturnsNull()
        {
            string json;
            using(var unitOfWork = Database.CreateUnitOfWork())
            {
                json = unitOfWork.GetByIdAsJson<IdentityItemForGetQueries>(1);
            }

            Assert.IsNull(json);
        }

        [Test]
        public void GetByGuidIdAsJson_WhenItemDoesNotExist_ReturnsNull()
        {
            var id = new Guid("b84f156a-139f-4add-b3d1-25e2b0696268");

            string json;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                json = unitOfWork.GetByIdAsJson<GuidItemForGetQueries>(id);
            }

            Assert.IsNull(json);
        }

        [Test]
        public void GetByGuidIdAsJson_WhenTwoItemsExistsAndTheSecondIsQueriedFor_ReturnsTheCorrectItemJson()
        {
            var id1 = new Guid("b84f156a-139f-4add-b3d1-25e2b0696268");
            var id2 = new Guid("8a2f9a21-d2fa-4eae-82ac-ace6aef34e7d");
            var item1 = new GuidItemForGetQueries { SisoId = id1, SortOrder = 1 };
            var item2 = new GuidItemForGetQueries { SisoId = id2, SortOrder = 2 };

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                unitOfWork.Insert(item1);
                unitOfWork.Insert(item2);
                unitOfWork.Commit();
            }

            string itemJsonRefetched;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                itemJsonRefetched = unitOfWork.GetByIdAsJson<GuidItemForGetQueries>(item2.SisoId);
            }

            Assert.AreEqual("{\"SisoId\":\"8a2f9a21d2fa4eae82acace6aef34e7d\",\"SortOrder\":2}", itemJsonRefetched);
        }

        [Test]
        public void GetAll_InsertInOneOrderReturnedInAnother_SortingIsCorrect()
        {
            var items = new List<IdentityItemForGetQueries>
                            {
                                new IdentityItemForGetQueries {SortOrder = 1, StringValue = "A"},
                                new IdentityItemForGetQueries {SortOrder = 5, StringValue = "E"},
                                new IdentityItemForGetQueries {SortOrder = 2, StringValue = "B"},
                                new IdentityItemForGetQueries {SortOrder = 4, StringValue = "D"},
                                new IdentityItemForGetQueries {SortOrder = 3, StringValue = "C"}
                            };

            IList<IdentityItemForGetQueries> refetched = null;
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                refetched = uow.GetAll<IdentityItemForGetQueries>(q => q.SortBy(i => i.SortOrder)).ToList();
            }

            CustomAssert.AreValueEqual(items[0], refetched[0]);
            CustomAssert.AreValueEqual(items[1], refetched[4]);
            CustomAssert.AreValueEqual(items[2], refetched[1]);
            CustomAssert.AreValueEqual(items[3], refetched[3]);
            CustomAssert.AreValueEqual(items[4], refetched[2]);
        }

        [Test]
        public void GetAllAs_InsertInOneOrderReturnedInAnother_SortingIsCorrect()
        {
            var items = new List<IdentityItemForGetQueries>
                            {
                                new IdentityItemForGetQueries {SortOrder = 1, StringValue = "A"},
                                new IdentityItemForGetQueries {SortOrder = 5, StringValue = "E"},
                                new IdentityItemForGetQueries {SortOrder = 2, StringValue = "B"},
                                new IdentityItemForGetQueries {SortOrder = 4, StringValue = "D"},
                                new IdentityItemForGetQueries {SortOrder = 3, StringValue = "C"}
                            };

            IList<ItemForQueriesInfo> refetched = null;
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                refetched = uow.GetAllAs<IdentityItemForGetQueries, ItemForQueriesInfo>(q => q.SortBy(i => i.StringValue)).ToList();
            }

            Assert.AreEqual(items[0].SortOrder, refetched[0].SortOrder);
            Assert.AreEqual(items[1].SortOrder, refetched[4].SortOrder);
            Assert.AreEqual(items[2].SortOrder, refetched[1].SortOrder);
            Assert.AreEqual(items[3].SortOrder, refetched[3].SortOrder);
            Assert.AreEqual(items[4].SortOrder, refetched[2].SortOrder);
        }

        [Test]
        public void GetAllAsJson_InsertInOneOrderReturnedInAnother_SortingIsCorrect()
        {
            var items = new List<IdentityItemForGetQueries>
                            {
                                new IdentityItemForGetQueries {SortOrder = 1, StringValue = "A"},
                                new IdentityItemForGetQueries {SortOrder = 5, StringValue = "E"},
                                new IdentityItemForGetQueries {SortOrder = 2, StringValue = "B"},
                                new IdentityItemForGetQueries {SortOrder = 4, StringValue = "D"},
                                new IdentityItemForGetQueries {SortOrder = 3, StringValue = "C"}
                            };

            IList<string> refetched = null;
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                refetched = uow.GetAllAsJson<IdentityItemForGetQueries>(q => q.SortBy(i => i.SortOrder)).ToList();
            }

            Assert.AreEqual("{\"SisoId\":1,\"SortOrder\":1,\"StringValue\":\"A\"}", refetched[0]);
            Assert.AreEqual("{\"SisoId\":2,\"SortOrder\":5,\"StringValue\":\"E\"}", refetched[4]);
            Assert.AreEqual("{\"SisoId\":3,\"SortOrder\":2,\"StringValue\":\"B\"}", refetched[1]);
            Assert.AreEqual("{\"SisoId\":4,\"SortOrder\":4,\"StringValue\":\"D\"}", refetched[3]);
            Assert.AreEqual("{\"SisoId\":5,\"SortOrder\":3,\"StringValue\":\"C\"}", refetched[2]);
        }

        [Test]
        public void GetAll_InsertInOneOrderReturnedInAnotherBySortingOnTwoMembers_SortingIsCorrect()
        {
            var items = new List<IdentityItemForGetQueries>
                            {
                                new IdentityItemForGetQueries {SortOrder = 2, StringValue = "B"},
                                new IdentityItemForGetQueries {SortOrder = 2, StringValue = "A"},
                                new IdentityItemForGetQueries {SortOrder = 1, StringValue = "B"},
                                new IdentityItemForGetQueries {SortOrder = 1, StringValue = "A"}
                            };

            IList<IdentityItemForGetQueries> refetched = null;
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                refetched = uow.Query<IdentityItemForGetQueries>(q => q.SortBy(i => i.StringValue, i => i.SortOrder)).ToList();
            }

            CustomAssert.AreValueEqual(items[0], refetched[3]);
            CustomAssert.AreValueEqual(items[1], refetched[1]);
            CustomAssert.AreValueEqual(items[2], refetched[2]);
            CustomAssert.AreValueEqual(items[3], refetched[0]);
        }

        private class IdentityItemForGetQueries
        {
            public int SisoId { get; set; }

            public int SortOrder { get; set; }

            public string StringValue { get; set; }
        }

        private class GuidItemForGetQueries
        {
            public Guid SisoId { get; set; }

            public int SortOrder { get; set; }

            public string StringValue { get; set; }
        }

        private class ItemForQueriesInfo
        {
            public int SortOrder { get; set; }
        }
    }
}