using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.Querying;
using SisoDb.TestUtils;

namespace SisoDb.Tests.IntegrationTests.Sql2008.UnitOfWork.Queries
{
    [TestFixture]
    public class Sql2008UnitOfWorkQueryWithPagingTests : Sql2008IntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<ItemForQueries>();
        }

        [Test]
        public void Query_Page_WhenMultiplePagesExists_CorrectPageStructuresAreReturned()
        {
            var items = new List<ItemForQueries>
                        {
                            new ItemForQueries{StringValue = "A"},
                            new ItemForQueries{StringValue = "B"},
                            new ItemForQueries{StringValue = "C"},
                            new ItemForQueries{StringValue = "D"},
                            new ItemForQueries{StringValue = "E"},
                            new ItemForQueries{StringValue = "F"},
                        };

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                var page1Result = uow.Query<ItemForQueries>(q => q.Page(0, 2)).ToList();
                var page2Result = uow.Query<ItemForQueries>(q => q.Page(1, 2)).ToList();

                Assert.AreEqual(2, page1Result.Count);
                Assert.AreEqual(2, page2Result.Count);
                CustomAssert.AreValueEqual(items[0], page1Result[0]);
                CustomAssert.AreValueEqual(items[1], page1Result[1]);
                CustomAssert.AreValueEqual(items[2], page2Result[0]);
                CustomAssert.AreValueEqual(items[3], page2Result[1]);
            }
        }

        [Test]
        public void Query_Page_WhenMultiplePagesExists_WithSpecificSorting_CorrectPageStructuresAreReturned()
        {
            var items = new List<ItemForQueries>
                        {
                            new ItemForQueries{StringValue = "A"},
                            new ItemForQueries{StringValue = "B"},
                            new ItemForQueries{StringValue = "C"},
                            new ItemForQueries{StringValue = "D"},
                            new ItemForQueries{StringValue = "E"},
                            new ItemForQueries{StringValue = "F"},
                        };

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                var page1Result = uow.Query<ItemForQueries>(q => q
                    .Page(0, 2)
                    .SortBy(i => i.StringValue.Desc())).ToList();
                var page2Result = uow.Query<ItemForQueries>(q => q
                    .Page(1, 2)
                    .SortBy(i => i.StringValue.Desc())).ToList();

                Assert.AreEqual(2, page1Result.Count);
                Assert.AreEqual(2, page2Result.Count);
                CustomAssert.AreValueEqual(items[5], page1Result[0]);
                CustomAssert.AreValueEqual(items[4], page1Result[1]);
                CustomAssert.AreValueEqual(items[3], page2Result[0]);
                CustomAssert.AreValueEqual(items[2], page2Result[1]);
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