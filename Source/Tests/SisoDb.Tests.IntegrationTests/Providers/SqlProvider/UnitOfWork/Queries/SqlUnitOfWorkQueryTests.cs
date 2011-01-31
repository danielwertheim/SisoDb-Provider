using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.Querying;
using SisoDb.TestUtils;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.Queries
{
    [TestFixture]
    public class SqlUnitOfWorkQueryTests : IntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<ItemForQueries>();
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
                    .OrderBy(c => c.SortOrder)
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
        public void QueryAsJosnIntByInterval_WhenInRange_ReturnsMatchingItems()
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
        public void QueryAsJosn_WhenQueryYieldsNoResult_NonNullEmptyResult()
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
    }
}