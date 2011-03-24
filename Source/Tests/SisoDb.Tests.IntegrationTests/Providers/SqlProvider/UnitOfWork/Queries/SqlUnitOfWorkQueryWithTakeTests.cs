using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.TestUtils;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.Queries
{
    [TestFixture]
    public class SqlUnitOfWorkQueryWithTakeTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<ItemForQueries>();
        }

        [Test]
        public void Query_Take2WithOutSorting_WhenThreeItemsExists_CorrectTwoItemsAreReturned()
        {
            var items = new List<ItemForQueries>
                        {
                            new ItemForQueries{StringValue = "C"},
                            new ItemForQueries{StringValue = "A"},
                            new ItemForQueries{StringValue = "B"}
                        };

            using(var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                var refetched = uow.Query<ItemForQueries>(q => q.Take(2)).ToList();

                Assert.AreEqual(2, refetched.Count);
                CustomAssert.AreValueEqual(items[0], refetched[0]);
                CustomAssert.AreValueEqual(items[1], refetched[1]);
            }
        }

        [Test]
        public void Query_Take2WithSorting_WhenThreeItemsExists_CorrectTwoItemsAreReturned()
        {
            var items = new List<ItemForQueries>
                        {
                            new ItemForQueries{StringValue = "C"},
                            new ItemForQueries{StringValue = "A"},
                            new ItemForQueries{StringValue = "B"}
                        };

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                var refetched = uow.Query<ItemForQueries>(q => q.Take(2).SortBy(i => i.StringValue)).ToList();

                Assert.AreEqual(2, refetched.Count);
                CustomAssert.AreValueEqual(items[1], refetched[0]);
                CustomAssert.AreValueEqual(items[2], refetched[1]);
            }
        }

        private class ItemForQueries
        {
            public int SisoId { get; set; }

            public int SortOrder { get; set; }

            public string StringValue { get; set; }
        }
    }
}