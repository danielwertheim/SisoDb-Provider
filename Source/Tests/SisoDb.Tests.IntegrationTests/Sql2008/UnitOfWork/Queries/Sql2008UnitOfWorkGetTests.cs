using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests.Sql2008.UnitOfWork.Queries
{
    [TestFixture]
    public class Sql2008UnitOfWorkGetTests : Sql2008IntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<IdentityItemForGetQueries>();
            DropStructureSet<GuidItemForGetQueries>();
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
                itemRefetched = unitOfWork.GetById<IdentityItemForGetQueries>(item2.StructureId);
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
                itemRefetched = unitOfWork.GetById<GuidItemForGetQueries>(item2.StructureId);
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
                itemJsonRefetched = unitOfWork.GetByIdAsJson<IdentityItemForGetQueries>(item2.StructureId);
            }

            Assert.AreEqual("{\"StructureId\":2,\"SortOrder\":2}", itemJsonRefetched);
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
            var item1 = new GuidItemForGetQueries { SortOrder = 1 };
            var item2 = new GuidItemForGetQueries { SortOrder = 2 };

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                unitOfWork.Insert(item1);
                unitOfWork.Insert(item2);
                unitOfWork.Commit();
            }

            string itemJsonRefetched;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                itemJsonRefetched = unitOfWork.GetByIdAsJson<GuidItemForGetQueries>(item2.StructureId);
            }

            Assert.AreEqual("{\"StructureId\":\"" + item2.StructureId.ToString("N") + "\",\"SortOrder\":2}", itemJsonRefetched);
        }
                
        private class IdentityItemForGetQueries
        {
            public int StructureId { get; set; }

            public int SortOrder { get; set; }

            public string StringValue { get; set; }
        }

        private class GuidItemForGetQueries
        {
            public Guid StructureId { get; set; }

            public int SortOrder { get; set; }

            public string StringValue { get; set; }
        }

        private class ItemForQueriesInfo
        {
            public int SortOrder { get; set; }
        }
    }
}