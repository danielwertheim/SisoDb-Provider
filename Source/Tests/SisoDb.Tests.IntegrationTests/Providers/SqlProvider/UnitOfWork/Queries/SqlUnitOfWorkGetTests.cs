using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

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
                itemRefetched = unitOfWork.GetById<IdentityItemForGetQueries>(item2.Id);
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
                itemRefetched = unitOfWork.GetById<GuidItemForGetQueries>(item2.Id);
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
                itemJsonRefetched = unitOfWork.GetByIdAsJson<IdentityItemForGetQueries>(item2.Id);
            }

            Assert.AreEqual("{\"Id\":2,\"SortOrder\":2}", itemJsonRefetched);
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
            var item1 = new GuidItemForGetQueries { Id = id1, SortOrder = 1 };
            var item2 = new GuidItemForGetQueries { Id = id2, SortOrder = 2 };

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                unitOfWork.Insert(item1);
                unitOfWork.Insert(item2);
                unitOfWork.Commit();
            }

            string itemJsonRefetched;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                itemJsonRefetched = unitOfWork.GetByIdAsJson<GuidItemForGetQueries>(item2.Id);
            }

            Assert.AreEqual("{\"Id\":\"8a2f9a21d2fa4eae82acace6aef34e7d\",\"SortOrder\":2}", itemJsonRefetched);
        }

        private class IdentityItemForGetQueries
        {
            public int Id { get; set; }

            public int SortOrder { get; set; }
        }

        private class GuidItemForGetQueries
        {
            public Guid Id { get; set; }

            public int SortOrder { get; set; }
        }
    }
}